using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLitePCL;
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
        public ApiTcgPlayer? tcgplayer { get; set; }
    }
    class ApiTcgPlayer
    {
        public Dictionary<string, ApiPriceDetail>? prices { get; set; }
    }
    class ApiPriceDetail
    {
        public string? low { get; set; }
        public string? mid { get; set; }
        public string? high { get; set; }
        public string? market { get; set; }
        public string? directLow { get; set; }
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
       
    public async Task<CardData?> FindCardAsync(string cardName, string setId, string? cardNumber = null)
{
        var q = $"name:{cardName} set.id:{setId}";
    
        if (!string.IsNullOrWhiteSpace(cardNumber))
        {
            q += $" number:{cardNumber}";
        } 

        var url = $"https://api.pokemontcg.io/v2/cards?q={System.Net.WebUtility.UrlEncode(q)}";
    
        var resp = await _http.GetAsync(url);

        if (!resp.IsSuccessStatusCode)
        {
            return null;
        }
        var raw = await resp.Content.ReadAsStringAsync();
        var parsed = JsonConvert.DeserializeObject<CardApiResponse>(raw);
    
        if (parsed?.data == null || parsed.data.Count == 0)
        {
            return null;
        }
        int? num = null;
        var c = parsed.data[0];
        
        if (int.TryParse(c.number, out int parsedNum))
        {
            num = parsedNum;
        }
        int.TryParse(c.hp, out var hp);

        string? result = null;

        if (c.tcgplayer?.prices != null)
        {
            foreach (var kvp in c.tcgplayer.prices)
            {
                if(kvp.Value?.market != null)
                {
                    result = kvp.Value.market;
                    break;
                }
            }
        }  

        return new CardData
        {
            Name = c.name ?? "Unknown",
            Number = num?.ToString() ?? "0",
            Hp = hp,
            Type = (c.types != null && c.types.Count > 0) ? c.types[0] : "Colorless",
            Image = c.images?.large ?? c.images?.small ?? "",
            RetreatCost = 0,
            Set = setId,
            Prices  = result ?? "N/A"
            };
        }
    }
}