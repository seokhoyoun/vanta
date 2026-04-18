using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Services.Auth;

namespace Vanta.Pages.Account
{
    public class LogoutModel : PageModel
    {
#region Fields

        private readonly ILocalAccountAuthService mLocalAccountAuthService;

#endregion

#region Constructors

        public LogoutModel(ILocalAccountAuthService localAccountAuthService)
        {
            mLocalAccountAuthService = localAccountAuthService;
        }

#endregion

#region Public Methods

        public async Task<IActionResult> OnPost()
        {
            await mLocalAccountAuthService.SignOut(HttpContext);
            return RedirectToPage("/Account/Login");
        }

#endregion
    }
}
