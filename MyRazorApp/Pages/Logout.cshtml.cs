// **AI** these are my codes for my Razor Pages project. now i want to  to implement a simple login mechanism. 
// following are the tasks and requirements. dont generate unnecessary files if not needed.
// 1)User Information Source
// Store user login information in a file named users.json located under wwwroot/data/.
// You must define a User class (Models/User.cs) to match the structure of this JSONfile.
// Below is the UML representation of the User class:
// +--------------------+
// | User |
// +--------------------+
// | - Username : string|
// | - Password : string|
// | - Role : string|
// | - IsActive : bool |
// | - CreatedAt: DateTime |
// +--------------------+
// 2)Login Functionality
// When the login form is submitted, read the users from the JSON file.
// Check whether the given credentials match an active user in the list.
// Session and Cookies
// Upon successful login:
// ▪ Generate a simple token
// ▪ Store the following in the session:
// ▪ username
// ▪ token
// ▪ session_id (use HttpContext.Session.Id)
// ▪ Store the same values in cookies using the following cookie settings:
// ▪ Expires in 30 minutes
// ▪ HttpOnly = true
// ▪ Secure = true
// ▪ SameSite = Strict
// 3)Access Control
// On all protected pages, check whether the token, username, and session_id from
// cookies match those in the session.
// If both token and username values match between the session and cookie, then
// you may consider the login valid.
// If the check fails, use errors and warnings to say “username or password is
// incorrect.” Or something like this message.
// 4)Logout
// Create a logout button that clears the session and removes all cookies related tologin.
// Upon successful logout, the user should be redirected to the login page.
// 5)Redirection after Login
// Upon successful login, the user should be redirected to the table page you
// implemented last week. This redirection should be implemented in the OnPostAsync
// method inside Login.cshtml.cs.
// project without using built-in authentication.

// **AI** there is no need for the "You have been successfully logged out. Go to Login Page".
// the logout button should clear the session and remove all cookies related to login 
// and automatically redirect the user to the login page. 

// **AI** i want the logout button to appear on the top right withiout changing the position of this table:

// **AI** i want the whole login container to appear at the center in the login page 

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToPage("./Login");
        }

        public async Task<IActionResult> OnGet()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToPage("./Login");
        }
    }
}