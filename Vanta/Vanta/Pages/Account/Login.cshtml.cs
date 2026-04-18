using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Services.Auth;

namespace Vanta.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
#region Fields

        private readonly ILocalAccountAuthService mLocalAccountAuthService;

#endregion

#region Constructors

        public LoginModel(ILocalAccountAuthService localAccountAuthService)
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
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

#endregion

#region Public Methods

        public async Task<IActionResult> OnGet()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToPage("/Index");
            }

            bool hasAnyUsers = await mLocalAccountAuthService.HasAnyUsers(HttpContext.RequestAborted);
            if (!hasAnyUsers)
            {
                return RedirectToPage("/Account/Setup");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool hasAnyUsers = await mLocalAccountAuthService.HasAnyUsers(HttpContext.RequestAborted);
            if (!hasAnyUsers)
            {
                return RedirectToPage("/Account/Setup");
            }

            LocalAccountAuthResult result = await mLocalAccountAuthService.SignIn(
                HttpContext,
                LoginId,
                Password,
                HttpContext.RequestAborted);

            if (!result.IsSuccess)
            {
                if (result.Error == ELocalAccountAuthError.UserInactive)
                {
                    ModelState.AddModelError(string.Empty, "This account is inactive.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login ID or password.");
                }

                return Page();
            }

            if (result.MustChangePassword)
            {
                return RedirectToPage("/Account/ChangePassword");
            }

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToPage("/Index");
        }

#endregion
    }
}
