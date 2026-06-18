using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TMS.Infrastructure.Data
{
    public class TMSDbContextFactory : IDesignTimeDbContextFactory<TMSDbContext>
    {
        public TMSDbContext CreateDbContext(string[] args)
        {
            // Walk up from Infrastructure to solution root, then into API
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../TMS.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TMSDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new TMSDbContext(optionsBuilder.Options);
        }
    }
}