namespace Vanta.Services.Auth
{
    public interface ILocalAccountAuthService
    {
        Task<bool> HasAnyUsers(CancellationToken cancellationToken = default);

        Task<LocalAccountAuthResult> SignIn(
            HttpContext httpContext,
            string loginId,
            string password,
            CancellationToken cancellationToken = default);

        Task<LocalAccountAuthResult> CreateInitialAdmin(
            HttpContext httpContext,
            string loginId,
            string displayName,
            string password,
            string passwordConfirmation,
            CancellationToken cancellationToken = default);

        Task<LocalAccountAuthResult> ChangePassword(
            HttpContext httpContext,
            string userId,
            string currentPassword,
            string newPassword,
            string newPasswordConfirmation,
            CancellationToken cancellationToken = default);

        Task SignOut(HttpContext httpContext);
    }
}
