using System.Collections.Generic;

namespace TCG_COMPANION.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public ICollection<CardData> Cards { get; set; } = null!;

    }
}