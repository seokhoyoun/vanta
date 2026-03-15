using System.Collections.Generic;
using Vanta.Comm.Abstractions.Processes;

namespace Vanta.Comm.Infrastructure.Adapter.Factories
{
    public sealed class DelegateProcessRuntimeFactory : IProcessRuntimeFactory
    {
        private readonly Dictionary<string, Func<IProcessRuntime>> _factories;

        public DelegateProcessRuntimeFactory(IDictionary<string, Func<IProcessRuntime>> factories)
        {
            _factories = new Dictionary<string, Func<IProcessRuntime>>(factories, StringComparer.OrdinalIgnoreCase);
        }

        public bool TryCreate(string processKey, out IProcessRuntime? runtime)
        {
            Func<IProcessRuntime>? factory;

            if (_factories.TryGetValue(processKey, out factory))
            {
                runtime = factory();
                return true;
            }

            runtime = null;
            return false;
        }
    }
}
