using APIFilmes.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIFilmes.Context
{
    public class APIFilmesDbContext : IdentityDbContext
    {
        public APIFilmesDbContext(DbContextOptions<APIFilmesDbContext> config) : base(config)
        {

        }

        // Configurações dos DbSet para os objetos
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<LocacaoItem> LocacoesItem { get; set; }
    }
}
