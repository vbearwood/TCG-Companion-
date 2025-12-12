using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;

namespace TCG_COMPANION.Pages
{
    [AllowAnonymous]
    public class MessageModel : PageModel
    {
        private readonly UserContext _context;

        public MessageModel (UserContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string? DisplayName { get; set; }

        public async Task OnGet()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                DisplayName = user.DisplayName;
            }
        }
    }
}