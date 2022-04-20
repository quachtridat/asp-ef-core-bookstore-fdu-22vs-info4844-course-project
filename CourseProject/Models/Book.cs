using CourseProject.Types;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject.Models {
    [Table("Books")]
    public class Book {
        [Key]
        public int BookId { get; set; }

        public string ImageFileName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = "Book Title";

        [Required]
        public SemicolonSplitStringList Descriptions { get; set; } = new SemicolonSplitStringList { "Book Description" };

        [Required]
        public SemicolonSplitStringList Categories { get; set; } = new SemicolonSplitStringList { "Book Category" };

        [Required]
        public SemicolonSplitStringList Authors { get; set; } = new SemicolonSplitStringList { "Book Author" };

        [Required]
        public SemicolonSplitStringList Countries { get; set; } = new SemicolonSplitStringList { "Book Country" };

        [Required]
        public SemicolonSplitStringList Languages { get; set; } = new SemicolonSplitStringList { "Book Language" };

        [Required]
        [Range(0, int.MaxValue)]
        public int Pages { get; set; }

        public int Year { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }
    }
}
