using System.Collections;
using System.Runtime.CompilerServices;

namespace TCG_COMPANION.Models
{
    public class GeminiStrategy
    {
        public int Id { get; set; }
        public CardData Card { get; set; } = null!;
        public string Request { get; set; } = null!;
        public string Response { get; set; } = null!;
    } 
}