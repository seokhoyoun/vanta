namespace Vanta.Models
{
    public class ProjectEquipment
    {
        public string Id { get; set; } = string.Empty;

        public string ProjectId { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string StageName { get; set; } = string.Empty;

        public string PlatformName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
