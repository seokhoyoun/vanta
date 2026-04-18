using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Services.Auth;

namespace Vanta.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {
#region Fields

        private readonly ILocalAccountAuthService mLocalAccountAuthService;

#endregion

#region Public Properties

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string NewPasswordConfirmation { get; set; } = string.Empty;

#endregion

#region Constructors

        public ChangePasswordModel(ILocalAccountAuthService localAccountAuthService)
        {
            mLocalAccountAuthService = localAccountAuthService;
        }

#endregion

#region Public Methods

        public IActionResult OnGet()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            LocalAccountAuthResult result = await mLocalAccountAuthService.ChangePassword(
                HttpContext,
                userId,
                CurrentPassword,
                NewPassword,
                NewPasswordConfirmation,
                HttpContext.RequestAborted);

            if (!result.IsSuccess)
            {
                ApplyErrors(result);
                return Page();
            }

            return RedirectToPage("/Index");
        }

#endregion

#region Private Methods

        private void ApplyErrors(LocalAccountAuthResult result)
        {
            switch (result.Error)
            {
                case ELocalAccountAuthError.InvalidCredentials:
                    ModelState.AddModelError(nameof(CurrentPassword), "Current password is incorrect.");
                    break;
                case ELocalAccountAuthError.PasswordConfirmationMismatch:
                    ModelState.AddModelError(nameof(NewPasswordConfirmation), "Passwords do not match.");
                    break;
                case ELocalAccountAuthError.InvalidPassword:
                    ModelState.AddModelError(nameof(NewPassword), "Password must be at least 8 characters.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Unable to change password.");
                    break;
            }
        }

#endregion
    }
}
