using System.ComponentModel.DataAnnotations.Schema;
namespace TCG_COMPANION.Models
{
    public class CardData
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;    
        public string? Number { get; set; }
        public string Type { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string? Prices { get; set; } = null!;
        public int Hp { get; set; }
        public int RetreatCost { get; set; }
        public string Set { get; set; } = null!;
    }
}