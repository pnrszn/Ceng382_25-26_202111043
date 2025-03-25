using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
//     **AI**The Id property will be automatically incremented each time a new item is added to the list. The list
// will act like a simple in-memory database. 
    public class ClassInformationModel
    {
        private static int _idCounter = 1;

        public int Id { get; private set; }

        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required.")]
        [Range(1, 100, ErrorMessage = "Student Count must be between 1 and 100.")]
        public int StudentCount { get; set; }

        [StringLength(200, ErrorMessage = "Description can't be longer than 200 characters.")]
        public string Description { get; set; }

        public ClassInformationModel()
        {
            Id = _idCounter++;
        }

        public static void ResetCounter(int maxId)
        {
            _idCounter = maxId + 1;
        }
    }
}
