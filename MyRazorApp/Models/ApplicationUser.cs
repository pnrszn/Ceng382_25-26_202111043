// **AI** i want to add an authentication to this razor page project. 
//follow these steps (steps provided in the lecture notes) and integrate authentication into my codes. 
//make sure already-existing functionalities remain working:

using Microsoft.AspNetCore.Identity;

namespace MyRazorApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}