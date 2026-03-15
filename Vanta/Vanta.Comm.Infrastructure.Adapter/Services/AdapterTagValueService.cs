using System;
using Vanta.Comm.Abstractions.Repositories;
using Vanta.Comm.Abstractions.Services;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Infrastructure.Adapter.Memory;

namespace Vanta.Comm.Infrastructure.Adapter.Services
{
    public sealed class AdapterTagValueService : ITagValueService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IAdapterTagStore _tagStore;

        public AdapterTagValueService(IConfigurationRepository configurationRepository, IAdapterTagStore tagStore)
        {
            _configurationRepository = configurationRepository;
            _tagStore = tagStore;
        }

        public Task<string?> GetLinkedTagValueAsync(
            string tagName,
            bool direct = false,
            CancellationToken cancellationToken = default)
        {
            return _tagStore.GetTagValueAsync(tagName, direct, cancellationToken);
        }

        public Task<bool> SetLinkedTagValueAsync(
            string tagName,
            string tagValue,
            bool direct = true,
            CancellationToken cancellationToken = default)
        {
            return _tagStore.SetTagValueAsync(tagName, tagValue, direct, cancellationToken);
        }

        public async Task<bool> IsLinkedTagValueAsync(
            string tagName,
            string expectedValue,
            bool direct = false,
            CancellationToken cancellationToken = default)
        {
            string? current = await GetLinkedTagValueAsync(tagName, direct, cancellationToken).ConfigureAwait(false);
            return string.Equals(current, expectedValue, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> GetDefineTagValueAsync(
            int defineTagSequence,
            int index1 = -1,
            int index2 = -1,
            bool direct = false,
            CancellationToken cancellationToken = default)
        {
            _ = index1;
            _ = index2;

            DefineTagDefinition? defineTag =
                await _configurationRepository.GetDefineTagAsync(defineTagSequence, cancellationToken).ConfigureAwait(false);

            if (defineTag == null)
            {
                return null;
            }

            return await GetLinkedTagValueAsync(defineTag.LinkedTagName, direct, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> SetDefineTagValueAsync(
            int defineTagSequence,
            string tagValue,
            int index1 = -1,
            int index2 = -1,
            bool direct = true,
            CancellationToken cancellationToken = default)
        {
            _ = index1;
            _ = index2;

            DefineTagDefinition? defineTag =
                await _configurationRepository.GetDefineTagAsync(defineTagSequence, cancellationToken).ConfigureAwait(false);

            if (defineTag == null)
            {
                return false;
            }

            return await SetLinkedTagValueAsync(defineTag.LinkedTagName, tagValue, direct, cancellationToken).ConfigureAwait(false);
        }

        public Task<int[]> GetBlockMemoryAsync(
            string commandKey,
            string memoryHead,
            int startAddress,
            int length,
            CancellationToken cancellationToken = default)
        {
            return _tagStore.GetBlockMemoryAsync(commandKey, memoryHead, startAddress, length, cancellationToken);
        }

        public Task<bool> SetBlockMemoryAsync(
            string commandKey,
            string memoryHead,
            int startAddress,
            IReadOnlyList<int> values,
            CancellationToken cancellationToken = default)
        {
            return _tagStore.SetBlockMemoryAsync(commandKey, memoryHead, startAddress, values, cancellationToken);
        }
    }
}
