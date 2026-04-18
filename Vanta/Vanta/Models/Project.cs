using System.Collections.Generic;

namespace Vanta.Models
{
    public class Project
    {
        public string Id { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string TeamName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;

        public string OwnerUserId { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
