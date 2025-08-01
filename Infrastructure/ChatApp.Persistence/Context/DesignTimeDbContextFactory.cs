using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ChatApp.Persistence.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ChatAppDbContext>
    {
        public ChatAppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ChatAppDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new ChatAppDbContext(optionsBuilder.Options);
        }
    }
}