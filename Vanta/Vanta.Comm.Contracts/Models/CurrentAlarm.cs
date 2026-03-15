namespace Vanta.Comm.Contracts.Models
{
    public sealed class CurrentAlarm
    {
        public int AlarmStatus { get; set; }

        public bool IsPopup { get; set; }

        public int AlarmId { get; set; }

        public int ErrorCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Module { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string DateTimeText { get; set; } = string.Empty;
    }
}
