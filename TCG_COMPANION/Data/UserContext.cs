using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Models;

namespace TCG_COMPANION.Data
{   
    public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;

    }
}