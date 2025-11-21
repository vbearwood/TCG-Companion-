using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;
using TCG_COMPANION.Models;
using TCG_COMPANION.Utils;

namespace TCG_COMPANION.Pages
{
	[Microsoft.AspNetCore.Authorization.Authorize]
	public class DeckModel : PageModel
	{
		private readonly PokemonSetHolder _setHolder;
		private readonly DeckContext _context;
		private readonly GeminiService  _geminiService;
        private readonly IHttpClientFactory _httpClientFactory = null!;
		public DeckModel(DeckContext context, GeminiService geminiService, PokemonSetHolder setHolder, IHttpClientFactory httpClientFactory)
		{
			_context = context;
			_geminiService = geminiService;
            _httpClientFactory = httpClientFactory;
			_setHolder = setHolder;
		}

		public Deck? Deck { get; set; }
		public string? Message { get; set; }

		public async Task OnGetAsync()
		{
			var username = User.Identity!.Name;

			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);
			if (Deck == null)
			{
				Message = "No deck found for your account.";
			}
		}

		public async Task<IActionResult> OnPostAddCardAsync(string CardName, string SetName)
		{
			var username = User.Identity!.Name;
			if(username == null)
            {
                Message = "Must be logged on";
                return Page();
            }

			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

			if (Deck == null)
			{
                Deck = new Deck
				{
					UserName = username,
					Cards = new List<CardData>()
				};
                _context.Decks.Add(Deck);
			}
			else if(Deck.Cards.Count() >= 60)
            {
                Message = "To many cards in deck. Only need 60.";
				return Page();
            }

			if(!_setHolder.SetNameToId.TryGetValue(SetName, out var setId))
			{
				Message = "Invalid set name.";
				return Page();
			}
			
			var httpClient = _httpClientFactory.CreateClient("PokemonTCG");
			var cardLookup = new PokemonTcgCardLookup(httpClient);
			var cardData = await cardLookup.FindCardAsync(CardName, setId);

			if (cardData == null)
			{
				Message = "Card not found in the Pokemon TCG database."; 
				return Page();
			}

			_context.Cards.Add(cardData); 
			Deck.Cards.Add(cardData);

			await _context.SaveChangesAsync();
			Message = "Card added successfully!";

			return RedirectToPage();
		}
		public async Task<IActionResult> OnPostDeleteCardAsync(string CardName, string SetName)
        {
			var username = User.Identity!.Name;
			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

			if(Deck == null)
            {
                Message = "No Deck on your account.";
				return Page();
            }

			if(!_setHolder.SetNameToId.TryGetValue(SetName, out var setId))
    		{
        		Message = "Invalid set name.";
        		return Page();
    		}
			var cardName = CardName; 
			var setName = SetName;
			var card = Deck.Cards.FirstOrDefault(d => d.Name == cardName && d.Set == setId);

			if(card == null)
            {
                Message = "Card not found in deck.";
				return Page();
            }

			Deck.Cards.Remove(card);
			await _context.SaveChangesAsync();
			Message = "Card removed successfully";
            return RedirectToPage();
        }
		public async Task<IActionResult> OnPostClearDeckAsync()
		{
			var username = User.Identity!.Name;
			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

			if (Deck == null)
			{
				Message = "No deck found for your account.";
				return Page();
			}

			Deck.Cards.Clear();
			await _context.SaveChangesAsync();
			Message = "Deck cleared successfully!";

			return RedirectToPage();
		}
		public async Task<IActionResult> OnPostGenerateStrategyAsync([FromForm] string playStyle)
        {
            var username = User.Identity!.Name;
            Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

            if (Deck == null || Deck.Cards.Count == 0)
            {
                Message = "No deck found for your account or deck is empty.";
                return Page();
            }

            var prompt =string.Join("\n", Deck.Cards.Select(c => $"{c.Name} ({c.Set} {c.Number})"));

            Message = await _geminiService.GetChatResponseAsync(prompt);

            return Content(Message);
		}
	}
}