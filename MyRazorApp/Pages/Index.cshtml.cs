// **AI** i want to create a razor page by implementing these steps and requirements:
// 1)This task will be implemented on the Index page (Index.cshtml and Index.cshtml.cs).
// • Create a folder named Models inside the project folder (note: folder name is plural).
// • Inside this folder, create a class named ClassInformationModel.cs.
// • This class will store the following properties:
// o Id (auto-incremented)
// o ClassName
// o StudentCount
// o Description
// The Id property will be automatically incremented each time a new item is added to the list. The list
// will act like a simple in-memory database.
// 2)On the left side of the page, there will be a form that collects:
// o Class Name
// o Student Count
// o Description
// • On the right side, there will be a table that displays all the submitted class data.
// • The table will have the following columns:
// o Id
// o Class Name
// o Student Count
// o Description
// o Actions (Edit and Delete)
// The data should be validated and added to a static list each time the form is submitted. The data will
// then be displayed in the table. 
// 3)Use Bootstrap to create a responsive layout with two columns (form on the left, table on the
// right).
// • Use Razor Pages only; no JavaScript is allowed.
// • All operations (Add, Edit, Delete) must be handled using C# methods in the PageModel.
// • Form validation should be done using C# attributes like [Required], [Range], etc.
// • When editing, pre-fill the form with the selected item's data.
// • After deletion or editing, refresh the page and update the table accordingly.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> classes = new List<ClassInformationModel>();
        private static int nextId = 1;

        [BindProperty]
        public ClassInformationModel NewClass { get; set; }

        [BindProperty]
        public int EditId { get; set; }

        public List<ClassInformationModel> Classes => classes;

        public void OnGet(int? editId)
        {
            if (editId.HasValue)
            {
                var classToEdit = classes.FirstOrDefault(c => c.Id == editId.Value);
                if (classToEdit != null)
                {
                    NewClass = classToEdit;
                    EditId = editId.Value;
                }
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (EditId == 0)
            {
                NewClass.Id = nextId++;
                classes.Add(NewClass);
            }
            else
            {
                var existingClass = classes.FirstOrDefault(c => c.Id == EditId);
                if (existingClass != null)
                {
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = classes.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                classes.Remove(classToRemove);
            }

            return RedirectToPage();
        }
    }
}