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

// **AI** (given labwork 9 tasks and previous week's codes) where and how should i integrate these codes?

// **AI** i want to connect this razor project to database. how can i do that?

// **AI** i want deleted classes to remain in the database with IsActive=0. but not be visible in the list. only classes with IsActive=1 should be visible in the list. 
// make sure all remaining functionalities work exactly the same.

// **AI** i have removed the IsActive checkbox. can you make it so IsActive is automatically set to 1 when a new class is created?

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MyRazorApp.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using MyRazorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private const int PageSize = 5;
        private readonly SchoolDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public IndexModel(SchoolDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private bool IsLoggedIn()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionSessionId = HttpContext.Session.GetString("session_id");

            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            return !string.IsNullOrEmpty(sessionUsername) &&
                   !string.IsNullOrEmpty(sessionToken) &&
                   !string.IsNullOrEmpty(sessionSessionId) &&
                   sessionUsername == cookieUsername &&
                   sessionToken == cookieToken &&
                   sessionSessionId == cookieSessionId;
        }

        [BindProperty]
        public Class NewClass { get; set; } = new Class();

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public IList<Class> ClassList { get; set; } = new List<Class>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsLoggedIn())
            {
                return RedirectToPage("./Login");
            }

            IQueryable<Class> query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(c => c.ClassName.Contains(SearchString));
            }

            TotalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            TotalPages = Math.Max(1, TotalPages);

            CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

            ClassList = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!IsLoggedIn())
            {
                return RedirectToPage("./Login");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            if (EditId != 0)
            {
                var existingClass = await _context.Classes.FindAsync(EditId);
                if (existingClass != null)
                {
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                }
            }
            else
            {
                NewClass.IsActive = true;
                _context.Classes.Add(NewClass);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { SearchString, CurrentPage });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToPage("./Login");
            }

            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                classToDelete.IsActive = false; 
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { SearchString, CurrentPage });
        }

        public async Task<IActionResult> OnPostExportJson(string selectedColumns)
        {
            if (!IsLoggedIn())
            {
                return RedirectToPage("./Login");
            }

            List<string> columnsToExport = string.IsNullOrEmpty(selectedColumns)
                ? null
                : selectedColumns.Split(',').ToList();

            IQueryable<Class> filteredQuery = _context.Classes.Where(c => c.IsActive); 
            if (!string.IsNullOrEmpty(SearchString))
            {
                filteredQuery = filteredQuery.Where(c => c.ClassName.Contains(SearchString));
            }

            List<Class> filteredData = await filteredQuery.ToListAsync();
    
            var exportData = filteredData.Select(item =>
            {
                var exportItem = new Dictionary<string, object>();
                if (columnsToExport == null || columnsToExport.Contains("ClassName"))
                    exportItem["ClassName"] = item.ClassName;
                if (columnsToExport == null || columnsToExport.Contains("StudentCount"))
                    exportItem["StudentCount"] = item.StudentCount;
                if (columnsToExport == null || columnsToExport.Contains("Description"))
                    exportItem["Description"] = item.Description;
                return exportItem;
            }).ToList();

            string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });

            string fileName = $"classes_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string filePath = Path.Combine(_environment.ContentRootPath, "JSONs", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            try
            {
                System.IO.File.WriteAllText(filePath, json);
                TempData["ExportMessage"] = $"JSON file successfully exported to: {filePath}";
            }
            catch (Exception ex)
            {
                TempData["ExportMessage"] = $"Error exporting JSON: {ex.Message}";
            }

            return RedirectToPage("./Index", new { SearchString, CurrentPage });
        }

        // private static void GenerateSyntheticData(int count)
        // {
        //     if (_classes.Any()) return;

        //     var random = new Random();
        //     var classPrefixes = new[] { "Math", "Science", "History", "Art", "Music", "Physics", "Chemistry", "Biology", "Literature", "Geography" };
        //     var classSuffixes = new[] { "101", "102", "201", "202", "301", "302", "Advanced", "Beginner", "Intermediate", "Workshop" };

        //     for (int i = 0; i < count; i++)
        //     {
        //         _classes.Add(new ClassInformationModel
        //         {
        //             Id = _nextId++,
        //             ClassName = $"{classPrefixes[random.Next(classPrefixes.Length)]} {classSuffixes[random.Next(classSuffixes.Length)]} {random.Next(1, 5)}",
        //             StudentCount = random.Next(10, 51),
        //             Description = $"Description for class number {i + 1}. Focuses on core concepts and practical applications."
        //         });
        //     }
        // }
    }
}