namespace Vanta.Comm.Abstractions.Simulation
{
    public interface IDeviceMemoryMap
    {
        bool IsConnected { get; }

        void Connect();

        void Disconnect();

        int[] ReadWords(string memoryHead, int startAddress, int length);

        void WriteWords(string memoryHead, int startAddress, IReadOnlyList<int> values);

        int ReadBit(string memoryHead, int startAddress, int bitOffset);

        void WriteBit(string memoryHead, int startAddress, int bitOffset, int value);
    }
}
