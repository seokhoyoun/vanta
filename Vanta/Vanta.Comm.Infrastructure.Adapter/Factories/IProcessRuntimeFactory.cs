using Vanta.Comm.Abstractions.Processes;

namespace Vanta.Comm.Infrastructure.Adapter.Factories
{
    public interface IProcessRuntimeFactory
    {
        bool TryCreate(string processKey, out IProcessRuntime? runtime);
    }
}
