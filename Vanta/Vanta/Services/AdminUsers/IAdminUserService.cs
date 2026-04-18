using Vanta.Models;

namespace Vanta.Services.AdminUsers
{
    public interface IAdminUserService
    {
        Task<List<User>> GetUsers(CancellationToken cancellationToken = default);

        Task<AdminUserCommandResult> CreateUser(
            string loginId,
            string displayName,
            string password,
            string passwordConfirmation,
            bool isAdmin,
            CancellationToken cancellationToken = default);

        Task<AdminUserCommandResult> ResetPassword(
            string userId,
            string newPassword,
            string newPasswordConfirmation,
            CancellationToken cancellationToken = default);
    }
}
