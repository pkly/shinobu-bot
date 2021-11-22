using Microsoft.EntityFrameworkCore;
using Shinobu.Database.Entity.Command;
using Shinobu.Database.Entity.Profile;

namespace Shinobu.Database
{
    public class ShinobuDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ShinobuDbContext(DbContextOptions<ShinobuDbContext> options): base(options) {}
        
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
    }
}