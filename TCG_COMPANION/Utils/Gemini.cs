using System.Text; 
using System.Text.Json; 


namespace TCG_COMPANION.Utils 
{

	public class GeminiService 
	{ 
		private readonly HttpClient _httpClient; 
		private readonly string? _apiKey;

		public GeminiService(HttpClient httpClient, IConfiguration config) 
		{ 
			_apiKey = config["Gemini:ApiKey"];
			_httpClient = httpClient; 
		} 

		public async Task<string> GetChatResponseAsync(string deck) 
		{ 
			var requestBody = new 
			{ 
				contents = new[] 
				{ 
					new { 
						parts = new[] { 
							new { text = $@"Analyze this Pokémon TCG deck and provide strategic advice:

								{deck}

								Consider:
								- Type balance and synergy
									- Potential combos
									- Weaknesses to watch for
									- Suggested modifications
									- General play strategy

									Keep response under 500 characters." } 
						} 
					} 
				} 
			}; 

			var request = new HttpRequestMessage( 
					HttpMethod.Post, 
					"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent"); 

			request.Headers.Add("X-Goog-Api-Key", _apiKey); 
			request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"); 

			var response = await _httpClient.SendAsync(request); 

			if (!response.IsSuccessStatusCode) 
			{ 
				var error = await response.Content.ReadAsStringAsync(); 
				throw new Exception($"Gemini API Error: {response.StatusCode} - {error}"); 
			} 

			using var responseStream = await response.Content.ReadAsStreamAsync(); 
			using var doc = await JsonDocument.ParseAsync(responseStream); 

			var text = doc.RootElement 
				.GetProperty("candidates")[0] 
				.GetProperty("content") 
				.GetProperty("parts")[0] 
				.GetProperty("text") 
				.GetString(); 

			return text ?? "No response from Gemini"; 
		} 
	}
}


