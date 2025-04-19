using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        private readonly string _usersFilePath;

        public LoginModel(IWebHostEnvironment environment)
        {
            _usersFilePath = Path.Combine(environment.WebRootPath, "data", "users.json");
        }

        public IActionResult OnGet()
        {
            // If already logged in, redirect to the table page
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }

        public IActionResult OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Username and password are required.";
                return Page();
            }

            try
            {
                var usersJson = System.IO.File.ReadAllText(_usersFilePath);
                var users = JsonSerializer.Deserialize<List<User>>(usersJson);

                var user = users?.FirstOrDefault(u =>
                    u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == Password &&
                    u.IsActive);

                if (user != null)
                {
                    // Successful login
                    var token = GenerateSimpleToken();
                    var sessionId = HttpContext.Session.Id;

                    // Store in session
                    HttpContext.Session.SetString("username", user.Username);
                    HttpContext.Session.SetString("token", token);
                    HttpContext.Session.SetString("session_id", sessionId);

                    // Cookie settings
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddMinutes(30),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };

                    // Store in cookies
                    HttpContext.Response.Cookies.Append("username", user.Username, cookieOptions);
                    HttpContext.Response.Cookies.Append("token", token, cookieOptions);
                    HttpContext.Response.Cookies.Append("session_id", sessionId, cookieOptions);

                    // Redirect to the table page (Index page from last week)
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                    return Page();
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while trying to log in.";
                return Page();
            }
        }

        private string GenerateSimpleToken()
        {
            return Guid.NewGuid().ToString(); // A simple unique token
        }
    }
}