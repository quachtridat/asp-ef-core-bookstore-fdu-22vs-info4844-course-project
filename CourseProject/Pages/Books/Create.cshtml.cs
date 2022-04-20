#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CourseProject.Models;

namespace CourseProject.Pages.Books {
    public class CreateModel : PageModel {
        private readonly Data.CourseProjectAzureSqlContext _context;

        public CreateModel(Data.CourseProjectAzureSqlContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            return Page();
        }

        [BindProperty]
        public Book Book { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { ForceClearCache = true } );
        }
    }
}
