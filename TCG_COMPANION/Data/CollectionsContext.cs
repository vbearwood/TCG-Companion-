using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Models;
namespace TCG_COMPANION.Data
{
    public class CollectionsContext(DbContextOptions<CollectionsContext> options) : DbContext(options)
    {
        public DbSet<Collections> Collections { get; set; } = null!;
    }
}