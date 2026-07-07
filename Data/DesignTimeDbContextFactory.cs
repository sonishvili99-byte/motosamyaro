using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace vroom.Data
{
    /// <summary>
    /// Used only by `dotnet ef migrations` tooling to generate PostgreSQL migrations.
    /// At runtime the provider is chosen dynamically in Program.cs.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Dummy connection — only used to infer the PostgreSQL schema for migration generation.
            optionsBuilder.UseNpgsql("Host=localhost;Database=vroom_design;Username=postgres;Password=postgres");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
