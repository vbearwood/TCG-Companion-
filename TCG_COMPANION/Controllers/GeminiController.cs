using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class StrategyController : ControllerBase
{
    private readonly IDeckStrategyService _service;
    
    public StrategyController(IDeckStrategyService service)
    {
        _service = service;
    }
    
    [HttpPost("deck")]
    public async Task<ActionResult<string>> GetStrategy([FromBody] DeckRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Deck))
            return BadRequest("Deck content required");
        
        // Parse the deck JSON to make it more readable for Gemini
        string formattedDeck;
        try
        {
            var deckObj = JsonSerializer.Deserialize<JsonElement>(req.Deck);
            var cards = deckObj.GetProperty("cards");
            
            formattedDeck = "Deck contains:\n";
            foreach (var card in cards.EnumerateArray())
            {
                var name = card.TryGetProperty("name", out var n) ? n.GetString() : 
                          card.TryGetProperty("Name", out var n2) ? n2.GetString() : "Unknown";
                formattedDeck += $"- {name}\n";
            }
        }
        catch
        {
            formattedDeck = req.Deck; // Fallback to raw string
        }
        
        var result = await _service.GetDeckStrategyAsync(formattedDeck, ct);
        
        // Return as plain text, not JSON
        return Content(result, "text/plain");
    }
}

public class DeckRequest
{
    public string Deck { get; set; } = "";
}