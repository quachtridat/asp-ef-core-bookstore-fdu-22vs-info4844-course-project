#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseProject.Models;

namespace CourseProject.Pages.Books {
    public class DetailsModel : PageModel {
        private readonly Data.CourseProjectAzureSqlContext _context;

        public DetailsModel(Data.CourseProjectAzureSqlContext context) {
            _context = context;
        }

        public Book Book { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Book = await _context.Books.FirstOrDefaultAsync(m => m.BookId == id);

            if (Book == null) {
                return NotFound();
            }
            return Page();
        }
    }
}
