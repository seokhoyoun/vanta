namespace Vanta.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;

        public string LoginId { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TeamName { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; }

        public bool MustChangePassword { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public DateTime? LastSignedInUtc { get; set; }
    }
}
