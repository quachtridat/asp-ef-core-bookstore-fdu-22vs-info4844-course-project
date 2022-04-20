#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseProject.Models;
using CourseProject.Types;

namespace CourseProject.Pages.Books {
    public class EditModel : PageModel {
        private readonly Data.CourseProjectAzureSqlContext _context;

        public EditModel(Data.CourseProjectAzureSqlContext context) {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public string BookAuthors { get; set; }
        [BindProperty]
        public string BookDescriptions { get; set; }
        [BindProperty]
        public string BookCategories { get; set; }
        [BindProperty]
        public string BookCountries { get; set; }
        [BindProperty]
        public string BookLanguages { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Book = await _context.Books.FirstOrDefaultAsync(m => m.BookId == id);

            if (Book == null) {
                return NotFound();
            }

            BookAuthors = (string)Book.Authors;
            BookDescriptions = (string)Book.Descriptions;
            BookCategories = (string)Book.Categories;
            BookCountries = (string)Book.Countries;
            BookLanguages = (string)Book.Languages;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            Book.Authors = BookAuthors.ToSemicolonSplitStringList(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Book.Descriptions = BookDescriptions.ToSemicolonSplitStringList(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Book.Categories = BookCategories.ToSemicolonSplitStringList(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Book.Countries = BookCountries.ToSemicolonSplitStringList(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Book.Languages = BookLanguages.ToSemicolonSplitStringList(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Attach(Book).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!BookExists(Book.BookId)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { ForceClearCache = true });
        }

        private bool BookExists(int id) {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
