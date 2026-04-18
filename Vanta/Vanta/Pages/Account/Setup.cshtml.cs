using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Services.Auth;

namespace Vanta.Pages.Account
{
    [AllowAnonymous]
    public class SetupModel : PageModel
    {
#region Fields

        private readonly ILocalAccountAuthService mLocalAccountAuthService;

#endregion

#region Constructors

        public SetupModel(ILocalAccountAuthService localAccountAuthService)
        {
            mLocalAccountAuthService = localAccountAuthService;
        }

#endregion

#region Public Properties

        [BindProperty]
        [Required]
        [Display(Name = "Login ID")]
        public string LoginId { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirmation { get; set; } = string.Empty;

#endregion

#region Public Methods

        public async Task<IActionResult> OnGet()
        {
            bool hasAnyUsers = await mLocalAccountAuthService.HasAnyUsers(HttpContext.RequestAborted);
            if (hasAnyUsers)
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

            LocalAccountAuthResult result = await mLocalAccountAuthService.CreateInitialAdmin(
                HttpContext,
                LoginId,
                DisplayName,
                Password,
                PasswordConfirmation,
                HttpContext.RequestAborted);

            if (!result.IsSuccess)
            {
                if (result.Error == ELocalAccountAuthError.SetupAlreadyCompleted)
                {
                    return RedirectToPage("/Account/Login");
                }

                if (result.Error == ELocalAccountAuthError.PasswordConfirmationMismatch)
                {
                    ModelState.AddModelError(nameof(PasswordConfirmation), "Passwords do not match.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to create the initial administrator.");
                }

                return Page();
            }

            return RedirectToPage("/Index");
        }

#endregion
    }
}
