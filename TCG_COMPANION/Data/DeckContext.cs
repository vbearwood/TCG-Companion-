using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Models;

namespace TCG_COMPANION.Data
{
    public class DeckContext : DbContext
    {
        public DeckContext(DbContextOptions<DeckContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Deck> Decks { get; set; } = null!;
        public DbSet<CardData> Cards { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deck>().HasMany(d => d.Cards).WithMany().UsingEntity(j => j.ToTable("DeckCards"));
            base.OnModelCreating(modelBuilder);
        }
    }
}