#nullable disable

using CourseProject.Models;
using CourseProject.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseProject.Data {
    public class CourseProjectAzureSqlContext : DbContext {
        public CourseProjectAzureSqlContext(DbContextOptions<CourseProjectAzureSqlContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder modelConfigurationBuilder) {
            modelConfigurationBuilder
                .Properties<SemicolonSplitStringList>()
                .HaveConversion<SemicolonSplitStringConverter, HashSetStringListComparer>();
        }
    }

    public class SemicolonSplitStringConverter : ValueConverter<SemicolonSplitStringList, string> {
        public SemicolonSplitStringConverter() : base(stringList => (string)stringList, str => new SemicolonSplitStringList(str.Split(';', StringSplitOptions.None))) { }
    }

    public class HashSetStringListComparer : ValueComparer<SemicolonSplitStringList> {
        public HashSetStringListComparer() : base(
            (lhs, rhs) => new HashSet<string>(lhs!).SetEquals(new HashSet<string>(rhs!)),
            strings => strings.Aggregate(0, (aggregated, str) => HashCode.Combine(aggregated, str.GetHashCode())),
            strings => new SemicolonSplitStringList(strings)
        ) { }
    }
}
