// **AI** i applied the steps until the step 5, but i dont want to use login partial because i already have my login page.
// if you need any of my codes, i can share it with you if you provide me the file names.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    Username,
                    Password,
                    isPersistent: false,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToPage("./Index");
                }

                ErrorMessage = "Invalid login attempt.";
            }
            return Page();
        }
    }
}