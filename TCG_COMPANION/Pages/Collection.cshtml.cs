using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;
using TCG_COMPANION.Models;
using TCG_COMPANION.Utils;

namespace TCG_COMPANION.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CollectionModel : PageModel
    {
        private readonly PokemonSetHolder _setHolder;
        private readonly CollectionsContext _context;
        private readonly IHttpClientFactory _httpClientFactory = null!;
        public CollectionModel(CollectionsContext context, PokemonSetHolder setHolder, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _setHolder = setHolder;
        }

        public Collections? Collection { get; set; }
        public string? Message { get; set; }
        public async Task OnGetAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                Message = "User not logged in.";
                return;
            }

            Collection = await _context.Collections.Include(c => c.Cards).FirstOrDefaultAsync(c => c.UserName == username);

            if (Collection == null)
            {
                Message = "No collection found for your account.";
            }
        }
        public async Task<IActionResult> OnPostAddCardAsync(string CardName, string SetName, string CardNumber)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                Message = "User not logged in.";
                return Page();
            }
            Collection = await _context.Collections.Include(c => c.Cards).FirstOrDefaultAsync(c => c.UserName == username);

            if (Collection == null)
            {
                Collection = new Collections
                {
                    UserName = username,
                    Cards = new List<CardData>()
                };
                _context.Collections.Add(Collection);
            }

			if(!_setHolder.SetNameToId.TryGetValue(SetName, out var setId))
			{
				Message = "Invalid set name.";
				return Page();
            }

            var httpClient = _httpClientFactory.CreateClient("PokemonTCG");
            var cardLookup = new PokemonTcgCardLookup(httpClient);
            var cardData = await cardLookup.FindCardAsync(CardName, setId,string.IsNullOrWhiteSpace(CardNumber) ? null : CardNumber);

            if (cardData == null)
            {
                Message = $"Card '{CardName}' not found in set '{SetName}'.";
                return Page();
            }
            
            _context.Cards.Add(cardData);
            Collection.Cards.Add(cardData);
            await _context.SaveChangesAsync();
            Message = $"Card '{CardName}' added to your collection.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearCollectionAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                Message = "User not logged in.";
                return Page();
            }

            Collection = await _context.Collections.Include(c => c.Cards).FirstOrDefaultAsync(c => c.UserName == username);

            if (Collection == null)
            {
                Message = "No collection found for your account.";
                return Page();
            }

            Collection.Cards.Clear();
            await _context.SaveChangesAsync();
            Message = "Your collection has been cleared.";

            return RedirectToPage();
        }
    }
}