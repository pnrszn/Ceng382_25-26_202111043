// **AI** i want to add an authentication to this razor page project. 
//follow these steps (steps provided in the lecture notes) and integrate authentication into my codes. 
//make sure already-existing functionalities remain working:

using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("User")]
        public string? UserId { get; set; }  
        public ApplicationUser? User { get; set; }
    }
}