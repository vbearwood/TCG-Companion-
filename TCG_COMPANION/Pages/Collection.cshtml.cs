using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TCG_COMPANION.Data;
using TCG_COMPANION.Models;
using TCG_COMPANION.Utils;

namespace TCG_COMPANION.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CollectionModel : PageModel
    {
        private readonly PokemonSetHolder _setHolder;
        private readonly ILogger<Collections> _logger;
        private readonly CollectionsContext _context;
        private readonly IHttpClientFactory _httpClientFactory = null!;
        public CollectionModel(CollectionsContext context, PokemonSetHolder setHolder, IHttpClientFactory httpClientFactory, ILogger<Collections> logger )
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _setHolder = setHolder;
        }

        public Collections? Collection { get; set; }
        public string? Message { get; set; }
        public async Task OnGetAsync()
        {
            var username = User.Identity!.Name;

            Collection = await _context.Collections.Include(c => c.Cards).FirstOrDefaultAsync(c => c.UserName == username);

            if (Collection == null)
            {
                _logger.LogWarning("No collection found for your account {username}.", username);
            }
        }
        public async Task<IActionResult> OnPostAddCardAsync(string CardName, string SetName)
        {
            var username = User.Identity!.Name;

            if(username == null)
            {

                _logger.LogWarning("Must be logged on {username}", username);
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
                _logger.LogWarning("Invalid set name {SetName}.", SetName);
				return Page();
            }

            var httpClient = _httpClientFactory.CreateClient("PokemonTCG");
            var cardLookup = new PokemonTcgCardLookup(httpClient);
            var cardData = await cardLookup.FindCardAsync(CardName, setId);

            if (cardData == null)
            {
                _logger.LogWarning("Card {CardName} not found in set {SetName}.", CardName, SetName);
                return Page();
            }
            
            _context.Cards.Add(cardData);
            Collection.Cards.Add(cardData);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Card '{CardName}' added to your collection.", CardName);

            return RedirectToPage();
        }
		public async Task<IActionResult> OnPostDeleteCardAsync(string CardName, string SetName)
        {
			var username = User.Identity!.Name;
			Collection = await _context.Collections.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

			if(Collection == null)
            {
                _logger.LogWarning("No Collection on your account {Collection}.", Collection);
				return Page();
            }

			if(!_setHolder.SetNameToId.TryGetValue(SetName, out var setId))
    		{
                _logger.LogWarning("Invalid set name {SetName}", SetName);
        		return Page();
    		}

			var cardName = CardName; 
			var setName = SetName;
			var card = Collection.Cards.FirstOrDefault(c => c.Name == cardName && c.Set == setId);

			if(card == null)
            {
                _logger.LogWarning("Card not found in Collection {Collection}", Collection);
				return Page();
            }

			Collection.Cards.Remove(card);
			await _context.SaveChangesAsync();
            _logger.LogInformation("Card removed successfully");
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostClearCollectionAsync()
        {
            var username = User.Identity!.Name;

            Collection = await _context.Collections.Include(c => c.Cards).FirstOrDefaultAsync(c => c.UserName == username);

            if (Collection == null)
            {
                _logger.LogWarning("No collection found on your account");
                return Page();
            }

            Collection.Cards.Clear();
            await _context.SaveChangesAsync();
            _logger.LogInformation("Your collection has been cleared");

            return RedirectToPage();
        }
    }
}