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

// **AI** these are the codes for my razor page. i have id, class name, student count, description and edit/delete features as icons, also add and update class features. 
// you need to add the following new features without changing the already existing features.
// now i want to add filtering and pagination features. filtering will be done on the data list in the backend and will be written inside of OnGet methods.
// you will also create a new model class called ClassInformationTable. This model will store
// the filtered version of your main model and will be used to display data in the table. In this
// model, the ID should not be shown in the table, but the ID will still be used in the
// background for actions like edit, delete, or details.
// in addition to filtering, you are required to implement pagination. To properly test the
// pagination feature, you need to generate synthetic data. Make sure to create a list with at
// least 100 sample records so you can see how the pagination works across multiple pages.
// Tip: When a filter value changes, the form should submit automatically or the user should click a
// "Filter" button. This will trigger the OnGet method with the selected filter values passed as
// query parameters.
// give the updated codes according to the file names i have given you.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MyRazorApp.Models;
using System;
using MyRazorApp.Helpers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> _classes = new List<ClassInformationModel>();
        private static int _nextId = 1;

        private const int PageSize = 5;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        static IndexModel()
        {
            GenerateSyntheticData(100);
        }

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public List<ClassInformationTable> DisplayClasses { get; private set; } = new List<ClassInformationTable>();

        private static void GenerateSyntheticData(int count)
        {
            if (_classes.Any()) return;

            var random = new Random();
            var classPrefixes = new[] { "Math", "Science", "History", "Art", "Music", "Physics", "Chemistry", "Biology", "Literature", "Geography" };
            var classSuffixes = new[] { "101", "102", "201", "202", "301", "302", "Advanced", "Beginner", "Intermediate", "Workshop" };

            for (int i = 0; i < count; i++)
            {
                _classes.Add(new ClassInformationModel
                {
                    Id = _nextId++,
                    ClassName = $"{classPrefixes[random.Next(classPrefixes.Length)]} {classSuffixes[random.Next(classSuffixes.Length)]} {random.Next(1, 5)}",
                    StudentCount = random.Next(10, 51),
                    Description = $"Description for class number {i + 1}. Focuses on core concepts and practical applications."
                });
            }
        }

        public void OnGet(int? editId)
        {
            IQueryable<ClassInformationModel> query = _classes.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(c => c.ClassName.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
            }

            TotalCount = query.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

            var paginatedData = query.Skip((CurrentPage - 1) * PageSize)
                                     .Take(PageSize)
                                     .ToList();

            DisplayClasses = paginatedData.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();

            if (editId.HasValue)
            {
                var classToEdit = _classes.FirstOrDefault(c => c.Id == editId.Value);
                if (classToEdit != null)
                {
                    NewClass = new ClassInformationModel
                    {
                        Id = classToEdit.Id,
                        ClassName = classToEdit.ClassName,
                        StudentCount = classToEdit.StudentCount,
                        Description = classToEdit.Description
                    };
                    EditId = editId.Value;
                }
                else
                {
                    EditId = 0;
                    NewClass = new ClassInformationModel();
                }
            }
        }

        public IActionResult OnPostAdd()
        {
            if (int.TryParse(Request.Form["EditId"], out int postedEditId))
            {
                EditId = postedEditId;
            }
            else
            {
                EditId = 0;
            }

            bool isUpdate = EditId != 0;

            if (!ModelState.IsValid)
            {
                OnGet(isUpdate ? EditId : (int?)null);
                return Page();
            }

            if (!isUpdate)
            {
                NewClass.Id = _nextId++;
                _classes.Add(new ClassInformationModel
                {
                    Id = NewClass.Id,
                    ClassName = NewClass.ClassName,
                    StudentCount = NewClass.StudentCount,
                    Description = NewClass.Description
                });
            }
            else
            {
                var existingClass = _classes.FirstOrDefault(c => c.Id == EditId);
                if (existingClass != null)
                {
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                }
                else
                {
                    return RedirectToPage("./Index", new { SearchString = SearchString, CurrentPage = CurrentPage });
                }
            }

            int targetPage = isUpdate ? CurrentPage : (int)Math.Ceiling(_classes.Count / (double)PageSize);
            if (targetPage == 0) targetPage = 1;

            return RedirectToPage("./Index", new { SearchString = SearchString, CurrentPage = targetPage });
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = _classes.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                _classes.Remove(classToRemove);
            }

            return RedirectToPage("./Index", new { SearchString = SearchString, CurrentPage = CurrentPage });
        }

        public IActionResult OnPostExportJson(string selectedColumns)
        {
            List<string> columnsToExport = string.IsNullOrEmpty(selectedColumns)
                ? null
                : selectedColumns.Split(',').ToList();

            // Apply the current filter to the original _classes list
            IQueryable<ClassInformationModel> filteredQuery = _classes.AsQueryable();
            if (!string.IsNullOrEmpty(SearchString))
            {
                filteredQuery = filteredQuery.Where(c => c.ClassName.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
            }
            List<ClassInformationModel> filteredData = filteredQuery.ToList();

            // Select only the desired columns from the filtered data
            var exportData = filteredData.Select(item =>
            {
                var exportItem = new Dictionary<string, object>();
                if (columnsToExport == null || columnsToExport.Contains("ClassName")) exportItem["ClassName"] = item.ClassName;
                if (columnsToExport == null || columnsToExport.Contains("StudentCount")) exportItem["StudentCount"] = item.StudentCount;
                if (columnsToExport == null || columnsToExport.Contains("Description")) exportItem["Description"] = item.Description;
                return exportItem;
            }).ToList();

            string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });

            // Determine the file path
            string fileName = $"classes_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string filePath = Path.Combine(_environment.ContentRootPath, "JSONs", fileName);

            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Write the JSON to the file
            try
            {
                System.IO.File.WriteAllText(filePath, json);
                TempData["ExportMessage"] = $"JSON file successfully exported to: {filePath}";
            }
            catch (Exception ex)
            {
                TempData["ExportMessage"] = $"Error exporting JSON: {ex.Message}";
            }

            return RedirectToPage("./Index", new { SearchString = SearchString, CurrentPage = CurrentPage });
        }
    }
}