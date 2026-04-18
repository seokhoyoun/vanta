namespace Vanta.Models
{
    public class ProjectMember
    {
        public string Id { get; set; } = string.Empty;

        public string ProjectId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public EProjectMemberRole Role { get; set; } = EProjectMemberRole.Developer;

        public DateTime JoinedUtc { get; set; }
    }
}
