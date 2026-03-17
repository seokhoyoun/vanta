using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication
{
    public sealed class MelsecMc3EBinaryClient : IMelsecCommunicationClient
    {
        private const int DefaultConnectTimeoutMilliseconds = 5000;
        private const int MinimumMonitoringTimerUnits = 1;
        private const int MaximumMonitoringTimerUnits = 0xFFFF;
        private const int RequestSubheader = 0x0050;
        private const int ResponseSubheader = 0x00D0;
        private const int BatchReadCommand = 0x0401;
        private const int BatchWriteCommand = 0x1401;
        private const int WordUnitSubcommand = 0x0000;
        private const int BitUnitSubcommand = 0x0001;
        private const int DefaultRequestDestinationModuleIoNumber = 0x03FF;
        private const int DefaultPcNumber = 0xFF;

        private readonly object _syncRoot = new object();
        private readonly SemaphoreSlim _ioLock = new SemaphoreSlim(1, 1);

        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private DeviceDefinition? _device;

        public async Task OpenAsync(DeviceDefinition device, CancellationToken cancellationToken = default)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (string.IsNullOrWhiteSpace(device.DeviceIpAddress))
            {
                throw new InvalidOperationException("MELSEC device IP address is required.");
            }

            if (device.DevicePort <= 0)
            {
                throw new InvalidOperationException("MELSEC device port is required.");
            }

            await CloseAsync(cancellationToken).ConfigureAwait(false);

            TcpClient tcpClient = new TcpClient();
            int timeoutMilliseconds = ResolveTimeoutMilliseconds(device.Timeout);

            Task connectTask = tcpClient.ConnectAsync(device.DeviceIpAddress, device.DevicePort);
            Task timeoutTask = Task.Delay(timeoutMilliseconds, cancellationToken);
            Task completedTask = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

            if (completedTask != connectTask)
            {
                tcpClient.Dispose();
                throw new TimeoutException("Timed out while connecting to the MELSEC device.");
            }

            await connectTask.ConfigureAwait(false);

            NetworkStream networkStream = tcpClient.GetStream();
            networkStream.ReadTimeout = timeoutMilliseconds;
            networkStream.WriteTimeout = timeoutMilliseconds;

            lock (_syncRoot)
            {
                _tcpClient = tcpClient;
                _networkStream = networkStream;
                _device = device;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            TcpClient? tcpClient;
            NetworkStream? networkStream;

            lock (_syncRoot)
            {
                tcpClient = _tcpClient;
                networkStream = _networkStream;
                _tcpClient = null;
                _networkStream = null;
            }

            if (networkStream != null)
            {
                networkStream.Dispose();
            }

            if (tcpClient != null)
            {
                tcpClient.Dispose();
            }

            return Task.CompletedTask;
        }

        public async Task<int[]> ReadAsync(
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default)
        {
            if (length <= 0)
            {
                return Array.Empty<int>();
            }

            MelsecDeviceAccessSpec spec = GetRequiredAccessSpec(memoryHead);
            byte[] requestFrame = BuildBatchReadRequest(spec, startAddress, length);
            byte[] responseData = await SendAndReceiveAsync(requestFrame, cancellationToken).ConfigureAwait(false);

            if (spec.IsBitDevice)
            {
                return DecodeBitReadData(responseData, length);
            }

            return DecodeWordReadData(responseData, length);
        }

        public async Task<bool> WriteAsync(
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Count == 0)
            {
                return true;
            }

            MelsecDeviceAccessSpec spec = GetRequiredAccessSpec(memoryHead);
            byte[] requestFrame = BuildBatchWriteRequest(spec, startAddress, values);

            await SendAndReceiveAsync(requestFrame, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task<byte[]> SendAndReceiveAsync(byte[] requestFrame, CancellationToken cancellationToken)
        {
            await _ioLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                NetworkStream networkStream = GetRequiredStream();

                await networkStream.WriteAsync(requestFrame, 0, requestFrame.Length, cancellationToken).ConfigureAwait(false);
                await networkStream.FlushAsync(cancellationToken).ConfigureAwait(false);

                byte[] header = new byte[9];
                await ReadExactAsync(networkStream, header, 0, header.Length, cancellationToken).ConfigureAwait(false);

                int subheader = ReadUInt16LittleEndian(header, 0);
                if (subheader != ResponseSubheader)
                {
                    throw new MelsecMcProtocolException(
                        "Unexpected MELSEC response subheader: 0x" + subheader.ToString("X4", CultureInfo.InvariantCulture) + ".");
                }

                int bodyLength = ReadUInt16LittleEndian(header, 7);
                if (bodyLength < 2)
                {
                    throw new MelsecMcProtocolException("Invalid MELSEC response length.");
                }

                byte[] body = new byte[bodyLength];
                await ReadExactAsync(networkStream, body, 0, body.Length, cancellationToken).ConfigureAwait(false);

                int endCode = ReadUInt16LittleEndian(body, 0);
                if (endCode != 0)
                {
                    throw new MelsecMcProtocolException(
                        "MELSEC device returned end code 0x" + endCode.ToString("X4", CultureInfo.InvariantCulture) + ".");
                }

                int dataLength = body.Length - 2;
                if (dataLength == 0)
                {
                    return Array.Empty<byte>();
                }

                byte[] data = new byte[dataLength];
                Buffer.BlockCopy(body, 2, data, 0, dataLength);
                return data;
            }
            catch
            {
                await CloseAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
            finally
            {
                _ioLock.Release();
            }
        }

        private static async Task ReadExactAsync(
            NetworkStream networkStream,
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken)
        {
            int totalRead = 0;

            while (totalRead < count)
            {
                int bytesRead = await networkStream.ReadAsync(
                        buffer,
                        offset + totalRead,
                        count - totalRead,
                        cancellationToken)
                    .ConfigureAwait(false);

                if (bytesRead <= 0)
                {
                    throw new IOException("The MELSEC connection was closed while receiving data.");
                }

                totalRead += bytesRead;
            }
        }

        private byte[] BuildBatchReadRequest(MelsecDeviceAccessSpec spec, int startAddress, int points)
        {
            List<byte> body = new List<byte>();
            AppendUInt16LittleEndian(body, ResolveMonitoringTimerUnits());
            AppendUInt16LittleEndian(body, BatchReadCommand);
            AppendUInt16LittleEndian(body, spec.IsBitDevice ? BitUnitSubcommand : WordUnitSubcommand);
            AppendDeviceAddress(body, startAddress);
            body.Add(spec.DeviceCode);
            AppendUInt16LittleEndian(body, points);

            return BuildFrame(body);
        }

        private byte[] BuildBatchWriteRequest(MelsecDeviceAccessSpec spec, int startAddress, IReadOnlyList<int> values)
        {
            List<byte> body = new List<byte>();
            AppendUInt16LittleEndian(body, ResolveMonitoringTimerUnits());
            AppendUInt16LittleEndian(body, BatchWriteCommand);
            AppendUInt16LittleEndian(body, spec.IsBitDevice ? BitUnitSubcommand : WordUnitSubcommand);
            AppendDeviceAddress(body, startAddress);
            body.Add(spec.DeviceCode);
            AppendUInt16LittleEndian(body, values.Count);

            if (spec.IsBitDevice)
            {
                AppendBitWriteData(body, values);
            }
            else
            {
                AppendWordWriteData(body, values);
            }

            return BuildFrame(body);
        }

        private byte[] BuildFrame(List<byte> body)
        {
            DeviceDefinition device = GetRequiredDevice();
            List<byte> frame = new List<byte>();

            AppendUInt16LittleEndian(frame, RequestSubheader);
            frame.Add((byte)device.NetworkNumber);
            frame.Add(DefaultPcNumber);
            AppendUInt16LittleEndian(frame, DefaultRequestDestinationModuleIoNumber);
            frame.Add((byte)device.StationNumber);
            AppendUInt16LittleEndian(frame, body.Count);

            int index;
            for (index = 0; index < body.Count; index++)
            {
                frame.Add(body[index]);
            }

            byte[] bytes = new byte[frame.Count];

            for (index = 0; index < frame.Count; index++)
            {
                bytes[index] = frame[index];
            }

            return bytes;
        }

        private static void AppendBitWriteData(List<byte> body, IReadOnlyList<int> values)
        {
            int index = 0;

            while (index < values.Count)
            {
                int low = NormalizeBit(values[index]);
                int high = 0;

                if (index + 1 < values.Count)
                {
                    high = NormalizeBit(values[index + 1]);
                }

                byte packed = (byte)(low | (high << 4));
                body.Add(packed);
                index += 2;
            }
        }

        private static void AppendWordWriteData(List<byte> body, IReadOnlyList<int> values)
        {
            int index;

            for (index = 0; index < values.Count; index++)
            {
                AppendUInt16LittleEndian(body, values[index]);
            }
        }

        private static int[] DecodeBitReadData(byte[] data, int points)
        {
            int[] values = new int[points];
            int pointIndex = 0;
            int byteIndex;

            for (byteIndex = 0; byteIndex < data.Length; byteIndex++)
            {
                byte packed = data[byteIndex];

                if (pointIndex < points)
                {
                    values[pointIndex] = packed & 0x0F;
                    pointIndex++;
                }

                if (pointIndex < points)
                {
                    values[pointIndex] = (packed >> 4) & 0x0F;
                    pointIndex++;
                }
            }

            return values;
        }

        private static int[] DecodeWordReadData(byte[] data, int points)
        {
            int[] values = new int[points];
            int index;

            for (index = 0; index < points; index++)
            {
                int offset = index * 2;
                if (offset + 1 >= data.Length)
                {
                    throw new MelsecMcProtocolException("MELSEC response data length is smaller than the requested word count.");
                }

                values[index] = ReadUInt16LittleEndian(data, offset);
            }

            return values;
        }

        private static void AppendDeviceAddress(List<byte> buffer, int startAddress)
        {
            if (startAddress < 0 || startAddress > 0xFFFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(startAddress));
            }

            buffer.Add((byte)(startAddress & 0xFF));
            buffer.Add((byte)((startAddress >> 8) & 0xFF));
            buffer.Add((byte)((startAddress >> 16) & 0xFF));
        }

        private static void AppendUInt16LittleEndian(List<byte> buffer, int value)
        {
            buffer.Add((byte)(value & 0xFF));
            buffer.Add((byte)((value >> 8) & 0xFF));
        }

        private static int ReadUInt16LittleEndian(byte[] buffer, int offset)
        {
            return buffer[offset] | (buffer[offset + 1] << 8);
        }

        private static int NormalizeBit(int value)
        {
            if (value == 0)
            {
                return 0;
            }

            return 1;
        }

        private static int ResolveTimeoutMilliseconds(int configuredTimeout)
        {
            if (configuredTimeout > 0)
            {
                return configuredTimeout;
            }

            return DefaultConnectTimeoutMilliseconds;
        }

        private int ResolveMonitoringTimerUnits()
        {
            DeviceDefinition device = GetRequiredDevice();
            int timeoutMilliseconds = ResolveTimeoutMilliseconds(device.Timeout);
            int monitoringTimerUnits = timeoutMilliseconds / 250;

            if ((timeoutMilliseconds % 250) != 0)
            {
                monitoringTimerUnits++;
            }

            if (monitoringTimerUnits < MinimumMonitoringTimerUnits)
            {
                monitoringTimerUnits = MinimumMonitoringTimerUnits;
            }

            if (monitoringTimerUnits > MaximumMonitoringTimerUnits)
            {
                monitoringTimerUnits = MaximumMonitoringTimerUnits;
            }

            return monitoringTimerUnits;
        }

        private NetworkStream GetRequiredStream()
        {
            lock (_syncRoot)
            {
                if (_networkStream == null)
                {
                    throw new InvalidOperationException("MELSEC TCP connection is not opened.");
                }

                return _networkStream;
            }
        }

        private DeviceDefinition GetRequiredDevice()
        {
            lock (_syncRoot)
            {
                if (_device == null)
                {
                    throw new InvalidOperationException("MELSEC device definition is not initialized.");
                }

                return _device;
            }
        }

        private static MelsecDeviceAccessSpec GetRequiredAccessSpec(string memoryHead)
        {
            MelsecDeviceAccessSpec? spec;

            if (!MelsecDeviceAccessCatalog.TryGetSpec(memoryHead, out spec) || spec == null)
            {
                throw new InvalidOperationException("Unsupported MELSEC device head '" + memoryHead + "'.");
            }

            return spec;
        }
    }
}
