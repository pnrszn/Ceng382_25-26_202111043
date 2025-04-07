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

using System.ComponentModel.DataAnnotations;
namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Student Count must be greater than 0.")]
        public int StudentCount { get; set; }

        public string Description { get; set; }
    }
}