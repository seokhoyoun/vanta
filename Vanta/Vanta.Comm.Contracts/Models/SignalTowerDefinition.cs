namespace Vanta.Comm.Contracts.Models
{
    public sealed class SignalTowerDefinition
    {
        public int SignalIndex { get; set; }

        public string SignalDescription { get; set; } = string.Empty;

        public int SignalType { get; set; }

        public int RedLamp { get; set; }

        public int YellowLamp { get; set; }

        public int GreenLamp { get; set; }

        public int BlueLamp { get; set; }

        public int SoundLamp { get; set; }
    }
}
