using APIFilmes.Model;
using Microsoft.EntityFrameworkCore;

namespace APIFilmes.Context
{
    public class APIFilmesDbContext : DbContext
    {
        public APIFilmesDbContext(DbContextOptions<APIFilmesDbContext> config) : base(config)
        {

        }

        // Configurações dos DbSet para os objetos
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Filme> Filmes { get; set; }
    }
}
