namespace Vanta.Services.Auth
{
    public enum ELocalAccountAuthError
    {
        None = 0,
        InvalidCredentials = 1,
        UserInactive = 2,
        SetupAlreadyCompleted = 3,
        PasswordConfirmationMismatch = 4,
        InvalidPassword = 5,
    }

    public class LocalAccountAuthResult
    {
        public static LocalAccountAuthResult Success(bool mustChangePassword = false)
        {
            LocalAccountAuthResult result = new LocalAccountAuthResult();
            result.MustChangePassword = mustChangePassword;
            return result;
        }

        public static LocalAccountAuthResult Failure(ELocalAccountAuthError error)
        {
            LocalAccountAuthResult result = new LocalAccountAuthResult();
            result.Error = error;
            return result;
        }

        public bool IsSuccess => Error == ELocalAccountAuthError.None;

        public ELocalAccountAuthError Error { get; private set; }

        public bool MustChangePassword { get; private set; }
    }
}
