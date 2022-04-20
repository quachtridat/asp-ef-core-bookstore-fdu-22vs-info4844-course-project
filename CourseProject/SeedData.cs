using CourseProject.Data;
using CourseProject.Models;
using CourseProject.Properties;
using CourseProject.Types;

using Newtonsoft.Json;

namespace CourseProject {
    public static class SeedData {
        public class Schema {
            public string Author { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string ImageFileName { get; set; } = string.Empty;
            public string Language { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
            public int Pages { get; set; }
            public string Title { get; set; } = string.Empty;
            public int Year { get; set; }
            public IList<string> Descriptions { get; set; } = new List<string>();
            public IList<string> Categories { get; set; } = new List<string>();
        }
        public static void SeedFor(CourseProjectAzureSqlContext context, ILogger? logger) {
            if (context == null) return;

            context.Database.EnsureCreated();

            if (context.Books.FirstOrDefault() != null) {
                logger?.LogDebug("Not seeding data because data already existed in the DB!");
                return;
            }

            var list = JsonConvert.DeserializeObject<Schema[]>(Resources.SeedDataRawJson);

            if (list == null) return;

            foreach (var item in list) {
                if (item == null) continue;

                logger?.LogInformation($"Seeding data for book: Title=\"{item.Title}\"...");

                context.Add(new Book {
                    Title = item.Title,
                    Authors = new SemicolonSplitStringList(item.Author.Split('/')),
                    Categories = new SemicolonSplitStringList(item.Categories),
                    Descriptions = new SemicolonSplitStringList(item.Descriptions),
                    Countries = new SemicolonSplitStringList(item.Country.Split('/')),
                    ImageFileName = item.ImageFileName,
                    Languages = new SemicolonSplitStringList(item.Language.Split('/')),
                    Pages = item.Pages,
                    Year = item.Year,
                    Price = (DateTime.Today.Year - item.Year) / 10f + item.Pages * 0.05f
                });
            }

            context.SaveChanges();
        }
    }
}