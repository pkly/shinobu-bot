using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shinobu.Database
{
    /// <summary>
    /// This class is only used for migration creation
    /// </summary>
    public class DbContextDesignFactory : IDesignTimeDbContextFactory<ShinobuDbContext>
    {
        public ShinobuDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ShinobuDbContext>();
            builder.UseMySql(
                "server=localhost;user=root;password=root;database=root", 
                new MariaDbServerVersion(new Version(10, 5, 5))
            );

            return new ShinobuDbContext(builder.Options);
        }
    }
}