// **AI** these are my codes. now add the following new features based on the requirements and according to the files names i have given you. if needed in the requirements, you can generate new files:
// Add a new button to export the data to JSON. There should be two modes:
// o Unfiltered export (exports the entire data)
// o Filtered export (exports only the currently filtered rows)
// • Also add the ability to select specific columns for export:
// o If no column is selected, export all columns.
// o If certain columns (e.g., 1st and 4th) are selected, export only those columns.
// o The selected columns should visually change color to indicate selection.
// o The exported JSON should contain only the selected column data.
// 4. Utility Class for JSON Export
// • Create a new C# class file named Utils.cs.
// • Inside it, implement a generic method that can export any class to JSON.
// • The method should work with any model class.
// • This class must be implemented as a singleton, so it can be accessed from anywhere in the project.
// 5. Folder Structure Reminder (MVP)
// Since your project follows the MVP structure in a Razor Pages application:
// • Place the ClassInformationTable and related data models in the Models folder.
// • Place the Utils.cs class in a separate folder called Helpers or Utilities.
// • Place pagination logic, filtering logic, and UI-related code in the appropriate Pages folder.

using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace MyRazorApp.Helpers 
{
    public sealed class Utils
    {
        private static readonly Utils _instance = new Utils();

        private Utils() { }

        public static Utils Instance => _instance;

        public string ExportToJson<T>(IEnumerable<T> data, List<string> selectedColumns = null) where T : class
        {
            if (data == null || !data.Any())
            {
                return "[]";
            }

            var options = new JsonSerializerOptions { WriteIndented = true };

            if (selectedColumns == null || !selectedColumns.Any())
            {
                return JsonSerializer.Serialize(data, options);
            }
            else
            {
                var filteredData = data.Select(item =>
                {
                    var dictionary = new Dictionary<string, object>();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (selectedColumns.Contains(property.Name))
                        {
                            dictionary[property.Name] = property.GetValue(item);
                        }
                    }
                    return dictionary;
                });
                return JsonSerializer.Serialize(filteredData, options);
            }
        }
    }
}