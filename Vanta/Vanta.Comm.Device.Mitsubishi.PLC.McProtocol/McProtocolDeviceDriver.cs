using System;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Contracts.Enums;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Addressing;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Constants;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.DataTypes;
using Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Runtime;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol
{
    public sealed class McProtocolDeviceDriver : IDeviceDriver
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<int, MelsecBlockContext> _blocksBySequence =
            new Dictionary<int, MelsecBlockContext>();

        private readonly Dictionary<int, MelsecTagContext> _tagsBySequence =
            new Dictionary<int, MelsecTagContext>();

        private readonly IMelsecCommunicationClient _communicationClient;
        private readonly MelsecAddressParser _addressParser;

        private DeviceDefinition? _device;
        private bool _isStarted;

        public McProtocolDeviceDriver()
            : this(new MelsecMc3EBinaryClient(), new MelsecAddressParser())
        {
        }

        public McProtocolDeviceDriver(
            IMelsecCommunicationClient communicationClient,
            MelsecAddressParser addressParser)
        {
            if (communicationClient == null)
            {
                throw new ArgumentNullException(nameof(communicationClient));
            }

            if (addressParser == null)
            {
                throw new ArgumentNullException(nameof(addressParser));
            }

            _communicationClient = communicationClient;
            _addressParser = addressParser;
        }

        public string DriverKey
        {
            get
            {
                return McProtocolDriverKeys.McProtocol;
            }
        }

        public Task InitializeAsync(DeviceDefinition device, CancellationToken cancellationToken = default)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            lock (_syncRoot)
            {
                _device = device;
            }

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            DeviceDefinition device = GetRequiredDevice();

            lock (_syncRoot)
            {
                if (_isStarted)
                {
                    return;
                }
            }

            await _communicationClient.OpenAsync(device, cancellationToken).ConfigureAwait(false);

            lock (_syncRoot)
            {
                _isStarted = true;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                if (!_isStarted)
                {
                    return;
                }
            }

            await _communicationClient.CloseAsync(cancellationToken).ConfigureAwait(false);

            lock (_syncRoot)
            {
                _isStarted = false;
            }
        }

        public Task ApplyBlockAsync(BlockDefinition block, CancellationToken cancellationToken = default)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            MelsecAddress baseAddress =
                _addressParser.Parse(block.BlockHead, block.BaseAddress, block.AddressFormat);

            int length = block.BlockLength;
            if (length <= 0)
            {
                length = 1;
            }

            MelsecBlockContext context = new MelsecBlockContext(
                block.BlockSequence,
                baseAddress.MemoryHead,
                baseAddress.Address,
                length,
                block.MemoryKind,
                block.AddressFormat);

            lock (_syncRoot)
            {
                _blocksBySequence[block.BlockSequence] = context;
            }

            return Task.CompletedTask;
        }

        public Task RemoveBlockAsync(int blockSequence, CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                _blocksBySequence.Remove(blockSequence);
            }

            return Task.CompletedTask;
        }

        public Task ApplyTagAsync(TagDefinition tag, CancellationToken cancellationToken = default)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            MelsecTagContext context = BuildTagContext(tag);

            lock (_syncRoot)
            {
                _tagsBySequence[tag.TagSequence] = context;
            }

            return Task.CompletedTask;
        }

        public Task RemoveTagAsync(int tagSequence, CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                _tagsBySequence.Remove(tagSequence);
            }

            return Task.CompletedTask;
        }

        public async Task<string?> GetDirectTagValueAsync(TagDefinition tag, CancellationToken cancellationToken = default)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.MemoryKind == MemoryKind.Constant)
            {
                return tag.RawValue;
            }

            EnsureStarted();

            MelsecTagContext context = GetOrCreateTagContext(tag);

            if (context.MemoryKind == MemoryKind.Bit)
            {
                int[] values =
                    await _communicationClient.ReadAsync(context.MemoryHead, context.StartAddress, 1, cancellationToken)
                        .ConfigureAwait(false);

                int wordValue = 0;
                if (values.Length > 0)
                {
                    wordValue = values[0];
                }

                int bitValue = ExtractBit(wordValue, context.BitDigit);
                return MelsecTagValueCodec.ConvertBitValueToString(tag, bitValue);
            }

            int[] blockValues =
                await _communicationClient.ReadAsync(context.MemoryHead, context.StartAddress, context.WordLength, cancellationToken)
                    .ConfigureAwait(false);

            return MelsecTagValueCodec.ConvertWordsToTagValue(tag, blockValues);
        }

        public async Task<bool> SetDirectTagValueAsync(TagDefinition tag, string value, CancellationToken cancellationToken = default)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.IsReadOnly)
            {
                return false;
            }

            if (tag.MemoryKind == MemoryKind.Constant)
            {
                return false;
            }

            EnsureStarted();

            MelsecTagContext context = GetOrCreateTagContext(tag);

            if (context.MemoryKind == MemoryKind.Bit)
            {
                int bitValue;

                if (!MelsecTagValueCodec.TryConvertStringToBitValue(tag, value, out bitValue))
                {
                    return false;
                }

                int[] currentValues =
                    await _communicationClient.ReadAsync(context.MemoryHead, context.StartAddress, 1, cancellationToken)
                        .ConfigureAwait(false);

                int currentWord = 0;
                if (currentValues.Length > 0)
                {
                    currentWord = currentValues[0];
                }

                int nextWord = SetBit(currentWord, context.BitDigit, bitValue);
                int[] writeValues = new int[1];
                writeValues[0] = nextWord;

                return await _communicationClient.WriteAsync(
                        context.MemoryHead,
                        context.StartAddress,
                        writeValues,
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            int[] wordValues;
            if (!MelsecTagValueCodec.TryConvertTagValueToWords(tag, value, context.WordLength, out wordValues))
            {
                return false;
            }

            return await _communicationClient.WriteAsync(
                    context.MemoryHead,
                    context.StartAddress,
                    wordValues,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<int[]> ReadBlockMemoryAsync(
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default)
        {
            EnsureStarted();
            return _communicationClient.ReadAsync(memoryHead, startAddress, length, cancellationToken);
        }

        public Task<bool> WriteBlockMemoryAsync(
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default)
        {
            EnsureStarted();
            return _communicationClient.WriteAsync(memoryHead, startAddress, values, cancellationToken);
        }

        private MelsecTagContext BuildTagContext(TagDefinition tag)
        {
            MelsecBlockContext? blockContext = FindBlockContext(tag.BlockSequence);

            string memoryHead = tag.AddressHead;
            if (string.IsNullOrWhiteSpace(memoryHead))
            {
                if (blockContext != null)
                {
                    memoryHead = blockContext.MemoryHead;
                }
            }

            if (string.IsNullOrWhiteSpace(memoryHead))
            {
                throw new InvalidOperationException("MELSEC tag memory head is required.");
            }

            int startAddress = 0;

            if (!string.IsNullOrWhiteSpace(tag.TagAddress))
            {
                AddressFormat addressFormat = AddressFormat.Decimal;
                if (blockContext != null)
                {
                    addressFormat = blockContext.AddressFormat;
                }

                MelsecAddress address = _addressParser.Parse(memoryHead, tag.TagAddress, addressFormat);
                startAddress = address.Address;
            }
            else if (blockContext != null)
            {
                startAddress = blockContext.StartAddress + tag.AddressIndex;
            }
            else
            {
                startAddress = tag.AddressIndex;
            }

            int wordLength = MelsecTagValueCodec.ResolveWordLength(tag);

            return new MelsecTagContext(
                tag.TagSequence,
                memoryHead,
                startAddress,
                wordLength,
                tag.BitDigit,
                tag.MemoryKind);
        }

        private MelsecTagContext GetOrCreateTagContext(TagDefinition tag)
        {
            lock (_syncRoot)
            {
                MelsecTagContext? context;

                if (_tagsBySequence.TryGetValue(tag.TagSequence, out context))
                {
                    return context;
                }
            }

            MelsecTagContext created = BuildTagContext(tag);

            lock (_syncRoot)
            {
                _tagsBySequence[tag.TagSequence] = created;
            }

            return created;
        }

        private MelsecBlockContext? FindBlockContext(int blockSequence)
        {
            lock (_syncRoot)
            {
                MelsecBlockContext? context;

                if (_blocksBySequence.TryGetValue(blockSequence, out context))
                {
                    return context;
                }
            }

            return null;
        }

        private DeviceDefinition GetRequiredDevice()
        {
            lock (_syncRoot)
            {
                if (_device == null)
                {
                    throw new InvalidOperationException("MELSEC device is not initialized.");
                }

                return _device;
            }
        }

        private void EnsureStarted()
        {
            lock (_syncRoot)
            {
                if (!_isStarted)
                {
                    throw new InvalidOperationException("MELSEC device driver is not started.");
                }
            }
        }

        private static int ExtractBit(int wordValue, int bitDigit)
        {
            if (bitDigit < 0)
            {
                return 0;
            }

            return (wordValue >> bitDigit) & 0x1;
        }

        private static int SetBit(int wordValue, int bitDigit, int bitValue)
        {
            if (bitDigit < 0)
            {
                return wordValue;
            }

            if (bitValue == 0)
            {
                return wordValue & ~(1 << bitDigit);
            }

            return wordValue | (1 << bitDigit);
        }
    }
}
