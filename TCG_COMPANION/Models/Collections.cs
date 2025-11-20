using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCG_COMPANION.Models
{
    public class Collections
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public ICollection<CardData> Cards { get; set; } = new List<CardData>();
    }
}