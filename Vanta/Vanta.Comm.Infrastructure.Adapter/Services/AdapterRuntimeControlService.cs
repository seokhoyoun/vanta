using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vanta.Comm.Abstractions.Devices;
using Vanta.Comm.Abstractions.Processes;
using Vanta.Comm.Abstractions.Repositories;
using Vanta.Comm.Abstractions.Services;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Infrastructure.Adapter.Factories;

namespace Vanta.Comm.Infrastructure.Adapter.Services
{
    public sealed class AdapterRuntimeControlService : IRuntimeControlService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IDeviceDriverFactory _deviceDriverFactory;
        private readonly IProcessRuntimeFactory _processRuntimeFactory;
        private readonly ConcurrentDictionary<int, IDeviceDriver> _activeDevices =
            new ConcurrentDictionary<int, IDeviceDriver>();

        private readonly ConcurrentDictionary<int, IProcessRuntime> _activeProcesses =
            new ConcurrentDictionary<int, IProcessRuntime>();

        public AdapterRuntimeControlService(
            IConfigurationRepository configurationRepository,
            IDeviceDriverFactory deviceDriverFactory,
            IProcessRuntimeFactory processRuntimeFactory)
        {
            _configurationRepository = configurationRepository;
            _deviceDriverFactory = deviceDriverFactory;
            _processRuntimeFactory = processRuntimeFactory;
        }

        public async Task StartAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<DeviceDefinition> devices =
                await _configurationRepository.GetDevicesAsync(cancellationToken).ConfigureAwait(false);

            foreach (DeviceDefinition device in devices)
            {
                if (device.IsEnabled)
                {
                    await StartDeviceAsync(device.DeviceId, cancellationToken).ConfigureAwait(false);
                }
            }

            IReadOnlyList<ProcessDefinition> processes =
                await _configurationRepository.GetProcessesAsync(cancellationToken).ConfigureAwait(false);

            foreach (ProcessDefinition process in processes)
            {
                if (process.IsEnabled)
                {
                    await StartProcessAsync(process.ProcessId, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task StopAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<int> deviceIds = GetActiveDeviceIds();
            foreach (int deviceId in deviceIds)
            {
                await StopDeviceAsync(deviceId, cancellationToken).ConfigureAwait(false);
            }

            IReadOnlyList<int> processIds = GetActiveProcessIds();
            foreach (int processId in processIds)
            {
                await StopProcessAsync(processId, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task StartDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            DeviceDefinition device = await GetRequiredDeviceAsync(deviceId, cancellationToken).ConfigureAwait(false);
            IDeviceDriver? existing;

            if (_activeDevices.TryGetValue(deviceId, out existing) && existing != null)
            {
                await existing.StartAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            IDeviceDriver? driver;
            if (!_deviceDriverFactory.TryCreate(device.DriverModule, out driver))
            {
                throw new InvalidOperationException("No device driver factory is registered for '" + device.DriverModule + "'.");
            }

            if (driver == null)
            {
                throw new InvalidOperationException("Device driver factory returned null for '" + device.DriverModule + "'.");
            }

            await driver.InitializeAsync(device, cancellationToken).ConfigureAwait(false);
            await ApplyDeviceConfigurationAsync(driver, deviceId, cancellationToken).ConfigureAwait(false);
            await driver.StartAsync(cancellationToken).ConfigureAwait(false);

            _activeDevices[deviceId] = driver;
        }

        public async Task StopDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            IDeviceDriver? driver;

            if (_activeDevices.TryRemove(deviceId, out driver) && driver != null)
            {
                await driver.StopAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task StartProcessAsync(int processId, CancellationToken cancellationToken = default)
        {
            ProcessDefinition process = await GetRequiredProcessAsync(processId, cancellationToken).ConfigureAwait(false);
            IProcessRuntime? existing;

            if (_activeProcesses.TryGetValue(processId, out existing) && existing != null)
            {
                await existing.StartAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            IProcessRuntime? runtime;
            if (!_processRuntimeFactory.TryCreate(process.ProcessModule, out runtime))
            {
                throw new InvalidOperationException("No process runtime factory is registered for '" + process.ProcessModule + "'.");
            }

            if (runtime == null)
            {
                throw new InvalidOperationException("Process runtime factory returned null for '" + process.ProcessModule + "'.");
            }

            await runtime.InitializeAsync(process, cancellationToken).ConfigureAwait(false);
            await ApplyProcessConfigurationAsync(runtime, processId, cancellationToken).ConfigureAwait(false);
            await runtime.StartAsync(cancellationToken).ConfigureAwait(false);

            _activeProcesses[processId] = runtime;
        }

        public async Task StopProcessAsync(int processId, CancellationToken cancellationToken = default)
        {
            IProcessRuntime? runtime;

            if (_activeProcesses.TryRemove(processId, out runtime) && runtime != null)
            {
                await runtime.StopAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RefreshDeviceConfigurationAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            IDeviceDriver? driver;

            if (_activeDevices.TryGetValue(deviceId, out driver) && driver != null)
            {
                DeviceDefinition device = await GetRequiredDeviceAsync(deviceId, cancellationToken).ConfigureAwait(false);
                await driver.InitializeAsync(device, cancellationToken).ConfigureAwait(false);
                await ApplyDeviceConfigurationAsync(driver, deviceId, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RefreshBlockConfigurationAsync(int blockSequence, CancellationToken cancellationToken = default)
        {
            BlockDefinition? block = await _configurationRepository.GetBlockAsync(blockSequence, cancellationToken).ConfigureAwait(false);

            if (block == null)
            {
                return;
            }

            IDeviceDriver? driver;
            if (_activeDevices.TryGetValue(block.DeviceId, out driver) && driver != null)
            {
                await driver.ApplyBlockAsync(block, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RefreshTagConfigurationAsync(int tagSequence, CancellationToken cancellationToken = default)
        {
            TagDefinition? tag = await _configurationRepository.GetTagAsync(tagSequence, cancellationToken).ConfigureAwait(false);

            if (tag == null)
            {
                return;
            }

            IDeviceDriver? driver;
            if (_activeDevices.TryGetValue(tag.DeviceId, out driver) && driver != null)
            {
                await driver.ApplyTagAsync(tag, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RefreshProcessConfigurationAsync(int processId, CancellationToken cancellationToken = default)
        {
            IProcessRuntime? runtime;

            if (_activeProcesses.TryGetValue(processId, out runtime) && runtime != null)
            {
                ProcessDefinition process = await GetRequiredProcessAsync(processId, cancellationToken).ConfigureAwait(false);
                await runtime.InitializeAsync(process, cancellationToken).ConfigureAwait(false);
                await ApplyProcessConfigurationAsync(runtime, processId, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RefreshSequenceConfigurationAsync(int sequenceId, CancellationToken cancellationToken = default)
        {
            SequenceDefinition? sequence =
                await _configurationRepository.GetSequenceAsync(sequenceId, cancellationToken).ConfigureAwait(false);

            if (sequence == null)
            {
                return;
            }

            IProcessRuntime? runtime;
            if (_activeProcesses.TryGetValue(sequence.ProcessId, out runtime) && runtime != null)
            {
                await runtime.ApplySequenceAsync(sequence, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<DeviceDefinition> GetRequiredDeviceAsync(int deviceId, CancellationToken cancellationToken)
        {
            DeviceDefinition? device = await _configurationRepository.GetDeviceAsync(deviceId, cancellationToken).ConfigureAwait(false);

            if (device == null)
            {
                throw new InvalidOperationException("Device '" + deviceId + "' was not found.");
            }

            return device;
        }

        private async Task<ProcessDefinition> GetRequiredProcessAsync(int processId, CancellationToken cancellationToken)
        {
            ProcessDefinition? process =
                await _configurationRepository.GetProcessAsync(processId, cancellationToken).ConfigureAwait(false);

            if (process == null)
            {
                throw new InvalidOperationException("Process '" + processId + "' was not found.");
            }

            return process;
        }

        private async Task ApplyDeviceConfigurationAsync(
            IDeviceDriver driver,
            int deviceId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<BlockDefinition> blocks =
                await _configurationRepository.GetBlocksAsync(cancellationToken).ConfigureAwait(false);

            foreach (BlockDefinition block in blocks)
            {
                if (block.DeviceId == deviceId)
                {
                    await driver.ApplyBlockAsync(block, cancellationToken).ConfigureAwait(false);
                }
            }

            IReadOnlyList<TagDefinition> tags =
                await _configurationRepository.GetTagsAsync(cancellationToken).ConfigureAwait(false);

            foreach (TagDefinition tag in tags)
            {
                if (tag.DeviceId == deviceId)
                {
                    await driver.ApplyTagAsync(tag, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task ApplyProcessConfigurationAsync(
            IProcessRuntime runtime,
            int processId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<SequenceDefinition> sequences =
                await _configurationRepository.GetSequencesAsync(cancellationToken).ConfigureAwait(false);

            foreach (SequenceDefinition sequence in sequences)
            {
                if (sequence.ProcessId == processId)
                {
                    await runtime.ApplySequenceAsync(sequence, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private IReadOnlyList<int> GetActiveDeviceIds()
        {
            List<int> deviceIds = new List<int>();

            foreach (int deviceId in _activeDevices.Keys)
            {
                deviceIds.Add(deviceId);
            }

            return deviceIds;
        }

        private IReadOnlyList<int> GetActiveProcessIds()
        {
            List<int> processIds = new List<int>();

            foreach (int processId in _activeProcesses.Keys)
            {
                processIds.Add(processId);
            }

            return processIds;
        }
    }
}
