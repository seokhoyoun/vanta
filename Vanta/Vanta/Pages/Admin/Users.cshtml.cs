using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Models;
using Vanta.Services.AdminUsers;

namespace Vanta.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
#region Public Properties

        public List<User> Users { get; private set; } = new List<User>();

        [BindProperty]
        public CreateUserInput CreateInput { get; set; } = new CreateUserInput();

        [BindProperty]
        public ResetPasswordInput ResetInput { get; set; } = new ResetPasswordInput();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

#endregion

#region Fields

        private readonly IAdminUserService mAdminUserService;

#endregion

#region Constructors

        public UsersModel(IAdminUserService adminUserService)
        {
            mAdminUserService = adminUserService;
        }

#endregion

#region Public Methods

        public async Task OnGet()
        {
            await LoadUsers();
        }

        public async Task<IActionResult> OnPostCreate()
        {
            ModelState.Clear();
            if (!TryValidateModel(CreateInput, nameof(CreateInput)))
            {
                await LoadUsers();
                return Page();
            }

            AdminUserCommandResult result = await mAdminUserService.CreateUser(
                CreateInput.LoginId,
                CreateInput.DisplayName,
                CreateInput.Password,
                CreateInput.PasswordConfirmation,
                CreateInput.IsAdmin,
                HttpContext.RequestAborted);

            if (!result.IsSuccess)
            {
                ApplyCreateErrors(result);
                await LoadUsers();
                return Page();
            }

            StatusMessage = "User account created.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResetPassword()
        {
            ModelState.Clear();
            if (!TryValidateModel(ResetInput, nameof(ResetInput)))
            {
                await LoadUsers();
                return Page();
            }

            AdminUserCommandResult result = await mAdminUserService.ResetPassword(
                ResetInput.UserId,
                ResetInput.NewPassword,
                ResetInput.NewPasswordConfirmation,
                HttpContext.RequestAborted);

            if (!result.IsSuccess)
            {
                ApplyResetErrors(result);
                await LoadUsers();
                return Page();
            }

            StatusMessage = "Password reset completed.";
            return RedirectToPage();
        }

#endregion

#region Private Methods

        private void ApplyCreateErrors(AdminUserCommandResult result)
        {
            switch (result.Error)
            {
                case EAdminUserCommandError.LoginIdAlreadyExists:
                    ModelState.AddModelError("CreateInput.LoginId", "Login ID already exists.");
                    break;
                case EAdminUserCommandError.PasswordConfirmationMismatch:
                    ModelState.AddModelError("CreateInput.PasswordConfirmation", "Passwords do not match.");
                    break;
                case EAdminUserCommandError.InvalidPassword:
                    ModelState.AddModelError("CreateInput.Password", "Password must be at least 8 characters.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Unable to create the user.");
                    break;
            }
        }

        private void ApplyResetErrors(AdminUserCommandResult result)
        {
            switch (result.Error)
            {
                case EAdminUserCommandError.UserNotFound:
                    ModelState.AddModelError(string.Empty, "User was not found.");
                    break;
                case EAdminUserCommandError.PasswordConfirmationMismatch:
                    ModelState.AddModelError(string.Empty, "Reset password confirmation does not match.");
                    break;
                case EAdminUserCommandError.InvalidPassword:
                    ModelState.AddModelError(string.Empty, "Reset password must be at least 8 characters.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Unable to reset the password.");
                    break;
            }
        }

        private async Task LoadUsers()
        {
            Users = await mAdminUserService.GetUsers(HttpContext.RequestAborted);
        }

#endregion

#region Input Models

        public class CreateUserInput
        {
            [Required]
            [Display(Name = "Login ID")]
            public string LoginId { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            public string PasswordConfirmation { get; set; } = string.Empty;

            [Display(Name = "Administrator")]
            public bool IsAdmin { get; set; }
        }

        public class ResetPasswordInput
        {
            [Required]
            public string UserId { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string NewPasswordConfirmation { get; set; } = string.Empty;
        }

#endregion
    }
}
