namespace Vanta.Comm.Abstractions.Services
{
    public interface ISharedMemoryStore
    {
        bool TryGetString(string memoryName, int index, out string? value);

        void SetString(string memoryName, int index, string value);

        bool TryGetInt32(string memoryName, int index, out int value);

        void SetInt32(string memoryName, int index, int value);
    }
}
