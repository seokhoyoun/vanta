using Microsoft.AspNetCore.Identity;
using Vanta.Models;
using Vanta.Repositories.Users;

namespace Vanta.Services.AdminUsers
{
    public class AdminUserService : IAdminUserService
    {
#region Fields

        private readonly IUserRepository mUserRepository;
        private readonly PasswordHasher<User> mPasswordHasher;

#endregion

#region Constructors

        public AdminUserService(IUserRepository userRepository)
        {
            mUserRepository = userRepository;
            mPasswordHasher = new PasswordHasher<User>();
        }

#endregion

#region Public Methods

        public Task<List<User>> GetUsers(CancellationToken cancellationToken = default)
        {
            return mUserRepository.GetAll(cancellationToken);
        }

        public async Task<AdminUserCommandResult> CreateUser(
            string loginId,
            string displayName,
            string password,
            string passwordConfirmation,
            bool isAdmin,
            CancellationToken cancellationToken = default)
        {
            if (!IsValidPassword(password))
            {
                return AdminUserCommandResult.Failure(EAdminUserCommandError.InvalidPassword);
            }

            if (!string.Equals(password, passwordConfirmation, StringComparison.Ordinal))
            {
                return AdminUserCommandResult.Failure(EAdminUserCommandError.PasswordConfirmationMismatch);
            }

            string normalizedLoginId = NormalizeLoginId(loginId);
            User? existingUser = await mUserRepository.GetByLoginIdOrNull(normalizedLoginId, cancellationToken);
            if (existingUser != null)
            {
                return AdminUserCommandResult.Failure(EAdminUserCommandError.LoginIdAlreadyExists);
            }

            User user = new User();
            user.LoginId = normalizedLoginId;
            user.DisplayName = displayName.Trim();
            user.Email = string.Empty;
            user.TeamName = string.Empty;
            user.IsAdmin = isAdmin;
            user.IsActive = true;
            user.MustChangePassword = true;
            user.CreatedUtc = DateTime.UtcNow;
            user.UpdatedUtc = DateTime.UtcNow;
            user.PasswordHash = mPasswordHasher.HashPassword(user, password);

            await mUserRepository.Create(user, cancellationToken);
            return AdminUserCommandResult.Success();
        }

        public async Task<AdminUserCommandResult> ResetPassword(
            string userId,
            string newPassword,
            string newPasswordConfirmation,
            CancellationToken cancellationToken = default)
        {
            if (!IsValidPassword(newPassword))
            {
                return AdminUserCommandResult.Failure(EAdminUserCommandError.InvalidPassword);
            }

            if (!string.Equals(newPassword, newPasswordConfirmation, StringComparison.Ordinal))
            {
                return AdminUserCommandResult.Failure(EAdminUserCommandError.PasswordConfirmationMismatch);
            }

            User? user = await mUserRepository.GetByIdOrNull(userId, cancellationToken);
            if (user == null)
            {
                return AdminUserCommandResult.Failure(EAdminUserCommandError.UserNotFound);
            }

            user.PasswordHash = mPasswordHasher.HashPassword(user, newPassword);
            user.MustChangePassword = true;
            user.UpdatedUtc = DateTime.UtcNow;
            await mUserRepository.Replace(user, cancellationToken);
            return AdminUserCommandResult.Success();
        }

#endregion

#region Private Methods

        private static bool IsValidPassword(string password)
        {
            return password.Trim().Length >= 8;
        }

        private static string NormalizeLoginId(string loginId)
        {
            return loginId.Trim().ToUpperInvariant();
        }

#endregion
    }
}
