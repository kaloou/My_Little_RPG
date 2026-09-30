using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Api.Models;

namespace MyLittleRPG.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pokemon> Pokemons { get; set; }
        
        public DbSet<Tuile> Tuiles { get; set; }
    }
}
