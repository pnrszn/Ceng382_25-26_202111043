using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{   
//     **AI**The Id property will be automatically incremented each time a new item is added to the list. The list
// will act like a simple in-memory database. 
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty]
        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            // En yüksek Id'yi alıp sayaçtan devam etmesini sağlıyoruz
            if (ClassList.Any())
            {
                ClassInformationModel.ResetCounter(ClassList.Max(c => c.Id));
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (IsEditing)
                {
                    // Edit modundaysa mevcut kaydı güncelle
                    var existingClass = ClassList.FirstOrDefault(c => c.Id == NewClass.Id);
                    if (existingClass != null)
                    {
                        existingClass.ClassName = NewClass.ClassName;
                        existingClass.StudentCount = NewClass.StudentCount;
                        existingClass.Description = NewClass.Description;
                    }
                }
                else
                {
                    // Yeni ekleme
                    ClassList.Add(NewClass);
                }

                // Formu sıfırla
                NewClass = new ClassInformationModel();
                IsEditing = false;
            }

            return Page();
        }

        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = classToEdit;
                IsEditing = true;
            }

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassList.Remove(classToRemove);
            }

            return Page();
        }
    }
}
