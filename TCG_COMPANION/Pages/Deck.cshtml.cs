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
		private readonly ILogger<DeckModel> _logger;

		public DeckModel(DeckContext context, GeminiService geminiService, PokemonSetHolder setHolder, IHttpClientFactory httpClientFactory, ILogger<DeckModel> logger)
		{
			_logger = logger;
			_context = context;
			_geminiService = geminiService;
            _httpClientFactory = httpClientFactory;
			_setHolder = setHolder;
		}

		public Deck? Deck { get; set; }
		public string? Message { get; set; }
		[BindProperty(SupportsGet = true)]
		public string? Search { get; set; }
		public ICollection<CardData> SearchDeck { get; set; } = default!;
		public async Task OnGetAsync()
		{
			var username = User.Identity!.Name;

			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);
			if (Deck == null)
			{
				_logger.LogWarning("No deck found for your account {username}.", username);
				SearchDeck = new List<CardData>();
			}
            else
            {
                var card = from i in Deck.Cards select i;
				if(!string.IsNullOrEmpty(Search))
            	{
               		card = card.Where(d => d.Name.Contains(Search)); 
            	}
				SearchDeck = card.ToList();
            }
		}

		public async Task<IActionResult> OnPostAddCardAsync(string CardName, string SetName, int CardNum, int Number)
		{
			var username = User.Identity!.Name;
			if(username == null)
            {
				_logger.LogWarning("Must be logged on {username}.", username);
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
				_logger.LogWarning("To many cards in deck. Only need 60 {username}.", username); 
				return Page();
            }

			if(!_setHolder.SetNameToId.TryGetValue(SetName, out var setId))
			{
				_logger.LogWarning("Invalid set name {SetName}", SetName);
				return Page();
			}
			
			var httpClient = _httpClientFactory.CreateClient("PokemonTCG");
			var cardLookup = new PokemonTcgCardLookup(httpClient);
			var cardData = await cardLookup.FindCardAsync(CardName, setId, CardNum);

			if (cardData == null)
			{
				_logger.LogWarning("Card {CardName} not found in set {SetName}.", CardName, SetName);
				return Page();
			}

			_context.Cards.Add(cardData); 
			Deck.Cards.Add(cardData);

			await _context.SaveChangesAsync();
			_logger.LogInformation("Card {CardName} added successfully.", CardName);

			return RedirectToPage();
		}
		public async Task<IActionResult> OnPostDeleteCardAsync(string CardName, string SetName, int CardNum)
        {
			var username = User.Identity!.Name;
			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

			if(Deck == null)
            {
				_logger.LogWarning("No Deck on your account {username}", username);
				return Page();
            }

			if(!_setHolder.SetNameToId.TryGetValue(SetName, out var setId))
    		{
				_logger.LogWarning("Invalid set name {SetName}", SetName);
        		return Page();
    		}
			
            var cardNum = CardNum.ToString();
			var card = Deck.Cards.FirstOrDefault(c => c.Name == CardName && c.Set == setId && c.Number == cardNum);

			if(card == null)
            {
				_logger.LogWarning("Card {CardName} not found in deck", CardName);
				return Page();
            }

			Deck.Cards.Remove(card);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Card {CardName} removed successfully.", CardName);
            return RedirectToPage();
        }
		public async Task<IActionResult> OnPostClearDeckAsync()
		{
			var username = User.Identity!.Name;
			Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

			if (Deck == null)
			{
				_logger.LogWarning("No deck found on your account {username}.", username);
				return Page();
			}

			Deck.Cards.Clear();
			await _context.SaveChangesAsync();
			_logger.LogInformation("Deck cleared successfully {username}", username);

			return RedirectToPage();
		}
		public async Task<IActionResult> OnPostGenerateStrategyAsync([FromForm] string playStyle)
        {
            var username = User.Identity!.Name;
            Deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == username);

            if (Deck == null || Deck.Cards.Count == 0)
            {
				_logger.LogWarning("No deck found on your account {username}", username);
                return Page();
            }

            var prompt =string.Join("\n", Deck.Cards.Select(c => $"{c.Name} ({c.Set} {c.Number})"));

            Message = await _geminiService.GetChatResponseAsync(prompt);

            return Content(Message);
		}
	}
}