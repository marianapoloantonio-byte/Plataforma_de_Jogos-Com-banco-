using Microsoft.EntityFrameworkCore;
using PlataformaDeJogosApi.Models;

namespace PlataformaDeJogosApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // --- Representa as tabelas para o banco de dados
        public DbSet<Usuario> Usuarios { get; set; } = null;
        public DbSet<Jogo> Jogos { get; set; } = null;
        public DbSet<Categoria> Categorias { get; set; } = null;
        public DbSet<Compra> Compras { get; set; } = null;
    }
}