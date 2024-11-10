using Microsoft.EntityFrameworkCore;
using Nostalgia_Games.Controllers;
using Nostalgia_Games.Data;
using Nostalgia_Games.Models;

namespace Nostalgia_Games.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Musica> Musicas { get; set; }
    }
}
