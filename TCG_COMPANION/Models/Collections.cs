using System.Collections.Generic;

namespace TCG_COMPANION.Models
{
    public class Collections
    {
        public int Id { get; set; }
        public User User { get; set; } = null!;
        public ICollection<CardData> Cards { get; set; } = new List<CardData>();

    }
}