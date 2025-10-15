using System.Text.Json;

public interface IDeckStrategyService
{
    Task<string> GetDeckStrategyAsync(string deck, CancellationToken ct = default);
}

public class DeckStrategyService : IDeckStrategyService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    
    public DeckStrategyService(IHttpClientFactory factory, IConfiguration cfg)
    {
        _http = factory.CreateClient("Gemini");
        _apiKey = cfg["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini:ApiKey not configured");
    }
    
    public async Task<string> GetDeckStrategyAsync(string deck, CancellationToken ct = default)
    {
        var prompt = $@"Analyze this Pokémon TCG deck and provide strategic advice:

{deck}

Consider:
- Type balance and synergy
- Potential combos
- Weaknesses to watch for
- Suggested modifications
- General play strategy

Keep response concise and practical.";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[] { new { text = prompt } }
                }
            }
        };
        
        try
        {
            var resp = await _http.PostAsJsonAsync(
                $"models/gemini-1.5-flash:generateContent?key={_apiKey}",
                body,
                ct);
            
            if (!resp.IsSuccessStatusCode)
            {
                var errorContent = await resp.Content.ReadAsStringAsync(ct);
                return $"API Error ({resp.StatusCode}): {errorContent}";
            }
            
            var json = await resp.Content.ReadAsStringAsync(ct);
            
            using var doc = JsonDocument.Parse(json);
            
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
            
            return text ?? "No response text generated.";
        }
        catch (Exception ex)
        {
            return $"Error generating strategy: {ex.Message}";
        }
    }
}
