using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Vanta.Models;
using Vanta.Repositories.Users;

namespace Vanta.Services.Auth
{
    public class LocalAccountAuthService : ILocalAccountAuthService
    {
#region Fields

        private readonly IUserRepository mUserRepository;
        private readonly PasswordHasher<User> mPasswordHasher;

#endregion

#region Constructors

        public LocalAccountAuthService(IUserRepository userRepository)
        {
            mUserRepository = userRepository;
            mPasswordHasher = new PasswordHasher<User>();
        }

#endregion

#region Public Methods

        public Task<bool> HasAnyUsers(CancellationToken cancellationToken = default)
        {
            return mUserRepository.HasAny(cancellationToken);
        }

        public async Task<LocalAccountAuthResult> SignIn(
            HttpContext httpContext,
            string loginId,
            string password,
            CancellationToken cancellationToken = default)
        {
            string normalizedLoginId = NormalizeLoginId(loginId);
            User? user = await mUserRepository.GetByLoginIdOrNull(normalizedLoginId, cancellationToken);

            if (user == null)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.InvalidCredentials);
            }

            if (!user.IsActive)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.UserInactive);
            }

            PasswordVerificationResult verificationResult = mPasswordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.InvalidCredentials);
            }

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = mPasswordHasher.HashPassword(user, password);
            }

            user.LastSignedInUtc = DateTime.UtcNow;
            user.UpdatedUtc = DateTime.UtcNow;
            await mUserRepository.Replace(user, cancellationToken);

            await SignInUser(httpContext, user);
            return LocalAccountAuthResult.Success(user.MustChangePassword);
        }

        public async Task<LocalAccountAuthResult> CreateInitialAdmin(
            HttpContext httpContext,
            string loginId,
            string displayName,
            string password,
            string passwordConfirmation,
            CancellationToken cancellationToken = default)
        {
            bool hasAnyUsers = await mUserRepository.HasAny(cancellationToken);
            if (hasAnyUsers)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.SetupAlreadyCompleted);
            }

            if (!string.Equals(password, passwordConfirmation, StringComparison.Ordinal))
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.PasswordConfirmationMismatch);
            }

            User user = new User();
            user.LoginId = NormalizeLoginId(loginId);
            user.DisplayName = displayName.Trim();
            user.Email = string.Empty;
            user.TeamName = string.Empty;
            user.IsAdmin = true;
            user.IsActive = true;
            user.CreatedUtc = DateTime.UtcNow;
            user.UpdatedUtc = DateTime.UtcNow;
            user.LastSignedInUtc = DateTime.UtcNow;
            user.PasswordHash = mPasswordHasher.HashPassword(user, password);

            await mUserRepository.Create(user, cancellationToken);
            await SignInUser(httpContext, user);
            return LocalAccountAuthResult.Success();
        }

        public Task SignOut(HttpContext httpContext)
        {
            return httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<LocalAccountAuthResult> ChangePassword(
            HttpContext httpContext,
            string userId,
            string currentPassword,
            string newPassword,
            string newPasswordConfirmation,
            CancellationToken cancellationToken = default)
        {
            User? user = await mUserRepository.GetByIdOrNull(userId, cancellationToken);
            if (user == null || !user.IsActive)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.InvalidCredentials);
            }

            PasswordVerificationResult verificationResult = mPasswordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                currentPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.InvalidCredentials);
            }

            if (!string.Equals(newPassword, newPasswordConfirmation, StringComparison.Ordinal))
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.PasswordConfirmationMismatch);
            }

            if (newPassword.Trim().Length < 8)
            {
                return LocalAccountAuthResult.Failure(ELocalAccountAuthError.InvalidPassword);
            }

            user.PasswordHash = mPasswordHasher.HashPassword(user, newPassword);
            user.MustChangePassword = false;
            user.LastSignedInUtc = DateTime.UtcNow;
            user.UpdatedUtc = DateTime.UtcNow;
            await mUserRepository.Replace(user, cancellationToken);
            await SignInUser(httpContext, user);
            return LocalAccountAuthResult.Success();
        }

#endregion

#region Private Methods

        private static string NormalizeLoginId(string loginId)
        {
            return loginId.Trim().ToUpperInvariant();
        }

        private static List<Claim> CreateClaims(User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));
            claims.Add(new Claim("login_id", user.LoginId));
            claims.Add(new Claim("must_change_password", user.MustChangePassword ? "true" : "false"));

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            return claims;
        }

        private static async Task SignInUser(HttpContext httpContext, User user)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                CreateClaims(user),
                CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            AuthenticationProperties properties = new AuthenticationProperties();
            properties.IsPersistent = true;
            properties.AllowRefresh = true;

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties);
        }

#endregion
    }
}
