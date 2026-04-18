namespace Vanta.Services.AdminUsers
{
    public enum EAdminUserCommandError
    {
        None = 0,
        UserNotFound = 1,
        LoginIdAlreadyExists = 2,
        PasswordConfirmationMismatch = 3,
        InvalidPassword = 4,
    }

    public class AdminUserCommandResult
    {
        public static AdminUserCommandResult Success()
        {
            return new AdminUserCommandResult();
        }

        public static AdminUserCommandResult Failure(EAdminUserCommandError error)
        {
            AdminUserCommandResult result = new AdminUserCommandResult();
            result.Error = error;
            return result;
        }

        public bool IsSuccess => Error == EAdminUserCommandError.None;

        public EAdminUserCommandError Error { get; private set; }
    }
}
