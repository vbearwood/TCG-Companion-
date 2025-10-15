using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace TCG_COMPANION.Models
{
    public class CardData
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;    
        public int Number { get; set; }
        public string Type { get; set; } = null!;
        public string Image { get; set; } = null!;
        [NotMapped]public Dictionary<string, string> Prices { get; set; } = null!;
        public int Hp { get; set; }
        [NotMapped]public List<string> Types { get; set; } = null!;
        [NotMapped]public Dictionary<string, string> Abilities { get; set; } = null!;
        [NotMapped]public Dictionary<string, string> Attacks { get; set; } = null!;
        [NotMapped]public Dictionary<string, string> Weaknesses { get; set; } = null!;
        [NotMapped]public List<string> Resistances { get; set; } = null!;
        public int RetreatCost { get; set; }
        [NotMapped]public List<string> Rules { get; set; } = null!;

    }
}