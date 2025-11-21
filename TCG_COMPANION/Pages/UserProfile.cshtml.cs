using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;

namespace TCG_COMPANION.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly UserContext _context;


        public UserProfileModel(UserContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string? DisplayName { get; set; }

        [BindProperty]
        public string? Bio { get; set; }



        public async Task OnGet()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                DisplayName = user.DisplayName;
                Bio = user.Bio;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Page();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            { 
                return Page();
            }
            user.DisplayName = DisplayName;
            user.Bio = Bio; 

            await _context.SaveChangesAsync();
            return RedirectToPage();

        }
    }
}