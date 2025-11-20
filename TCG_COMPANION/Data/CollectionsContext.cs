using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Models;

namespace TCG_COMPANION.Data
{
    public class CollectionsContext : DbContext
    {
        public CollectionsContext(DbContextOptions<CollectionsContext> options) : base(options) { }

        public DbSet<Collections> Collections { get; set; } = null!;
        public DbSet<CardData> Cards { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collections>()
                .HasMany(d => d.Cards)
                .WithMany()
                .UsingEntity(j => j.ToTable("CollectionCards"));

            base.OnModelCreating(modelBuilder);
        }
    }
}