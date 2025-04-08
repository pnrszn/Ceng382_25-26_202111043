using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace MyRazorApp.Helpers // Or MyRazorApp.Utilities
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