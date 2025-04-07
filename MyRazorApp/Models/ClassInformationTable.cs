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

namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        public string Description { get; set; }
    }
}