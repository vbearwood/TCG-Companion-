using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;
using TCG_COMPANION.Models;
using TCG_COMPANION.Utils;
using System.Text.Json; 
namespace TCG_COMPANION.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly DeckContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        public class CardInput
        {
            public string Name { get; set; } = null!;
            public string Set { get; set; } = null!;
            
            public string Number { get; set; } = null!;
        }
        public DecksController(DeckContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/Decks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Deck>>> GetDecks()
        {
            return await _context.Decks.Include(d => d.Cards).ToListAsync();
        }

        // GET: api/Decks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Deck>> GetDeck(int id)
        {
            var deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == id);
            if (deck == null)
            {
                return NotFound();
            }

            return deck;
        }

        // PUT: api/Decks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeck(int id, Deck deck)
        {
            if (id != deck.Id)
            {
                return BadRequest();
            }

            _context.Entry(deck).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeckExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Decks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
[HttpPost]
public async Task<ActionResult<Deck>> PostDeck(CardInput cardData)
{
    var cardName = cardData.Name;
    var setName = cardData.Set;
    var cardNumber = cardData.Number?.TrimStart('0');

    if (string.IsNullOrWhiteSpace(cardName) || string.IsNullOrWhiteSpace(setName) || string.IsNullOrWhiteSpace(cardNumber))
    {
        return BadRequest("Card Name, Set, and Number are required");
    }

    if (User?.Identity?.IsAuthenticated != true)
    {
        return Unauthorized("User must be logged in to create or modify a deck");
    }

    var httpClient = _httpClientFactory.CreateClient("PokemonTCG");
    var cards = new PokemonTcgCardLookup(httpClient);
    var card = await cards.FindCardAsync(cardData.Name, cardData.Set, cardData.Number);

    if (card == null)
    {
        return BadRequest("Card not found in the Pokemon TCG database");
    }

    // Use FirstOrDefaultAsync with username filter instead of FindAsync
    var deck = await _context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.UserName == User.Identity!.Name);

    if (deck == null)
    {
        var cardList = new List<CardData> { card };
        deck = new Deck { UserName = User.Identity!.Name!, Cards = cardList };
        _context.Decks.Add(deck);
    }
    else
    {
        deck.Cards.Add(card);
        _context.Entry(deck).State = EntityState.Modified;
    }

    await _context.SaveChangesAsync();

    return CreatedAtAction("GetDeck", new { id = deck.Id }, deck);
}
        
        // DELETE: api/Decks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeck(int id)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                return NotFound();
            }

            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.Id == id);
        }
    }
}
