using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TCG_COMPANION.Models;

namespace TCG_COMPANION.Utils
{
    class CardApiResponse
    {
        public System.Collections.Generic.List<ApiCard>? data { get; set; }
    }
   
    class ApiCard
    {
        public string? name { get; set; }
        public string? number { get; set; }
        public string? hp { get; set; }
        public System.Collections.Generic.List<string>? types { get; set; }
        public ApiImages? images { get; set; }
    }
   
    class ApiImages
    {
        public string? small { get; set; }
        public string? large { get; set; }
    }
   
    public class PokemonTcgCardLookup
    {
        private readonly HttpClient _http;
       
        public PokemonTcgCardLookup(HttpClient http)
        {
            _http = http;
        }
       
        public async Task<CardData?> FindCardAsync(string cardName, string setName, string? cardNumber = null)
        {
            var q = $"name:\"{cardName}\" set.name:\"{setName}\"";
            
            if (!string.IsNullOrWhiteSpace(cardNumber))
                q += $" number:{cardNumber}";
                
            var url = $"https://api.pokemontcg.io/v2/cards?q={q}";
           
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return null;
                
            var raw = await resp.Content.ReadAsStringAsync();
            var parsed = JsonConvert.DeserializeObject<CardApiResponse>(raw);
            
            if (parsed?.data == null || parsed.data.Count == 0)
                return null;
                
            var c = parsed.data[0];
            
            if (c == null)
                return null;
                
            int.TryParse(c.number, out var num);
            int.TryParse(c.hp, out var hp);
            
            return new CardData
            {
                Name = c.name ?? "Unknown",
                Number = num,
                Hp = hp,
                Type = (c.types != null && c.types.Count > 0) ? c.types[0] : "Colorless",
                Image = c.images?.large ?? c.images?.small ?? "",
                RetreatCost = 0
            };
        }
    }
}