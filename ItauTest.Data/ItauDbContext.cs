using ItauTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ItauTest.Data
{
    public class ItauDbContext : DbContext
    {
        public ItauDbContext(DbContextOptions<ItauDbContext> options) : base(options) { }

        public DbSet<Operacao> Operacoes { get; set; }
        public DbSet<Cotacao> Cotacoes { get; set; }
        public DbSet<Posicao> Posicoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }     // Novo
        public DbSet<Ativo> Ativos { get; set; }         // Novo

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da tabela Operacao
            modelBuilder.Entity<Operacao>(entity =>
            {
                entity.ToTable("operacao");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdUsuario).IsRequired().HasColumnName("id_usuario");
                entity.Property(e => e.AtivoId).IsRequired().HasColumnName("ativo_id");
                entity.Property(e => e.Quantidade).IsRequired().HasColumnName("quantidade");
                entity.Property(e => e.PrecoUnitario).IsRequired().HasPrecision(18, 4).HasColumnName("preco_unitario");
                entity.Property(e => e.DataHora).IsRequired().HasColumnName("data_hora");
                entity.Property(e => e.Corretagem).IsRequired().HasPrecision(18, 4).HasColumnName("corretagem");

                entity.HasIndex(e => new { e.IdUsuario, e.AtivoId, e.DataHora }).HasDatabaseName("idx_usuario_ativo_data");
            });

            // Configuração da tabela Cotacao
            modelBuilder.Entity<Cotacao>(entity =>
            {
                entity.ToTable("cotacao");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AtivoId).IsRequired().HasColumnName("ativo_id");
                entity.Property(e => e.PrecoUnitario).IsRequired().HasPrecision(18, 4).HasColumnName("preco_unitario");
                entity.Property(e => e.DataHora).IsRequired().HasColumnName("data_hora");

                entity.HasIndex(e => new { e.AtivoId, e.DataHora }).HasDatabaseName("idx_ativo_data");
            });

            // Configuração da tabela Posicao
            modelBuilder.Entity<Posicao>(entity =>
            {
                entity.ToTable("posicao");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdUsuario).IsRequired().HasColumnName("id_usuario");
                entity.Property(e => e.AtivoId).IsRequired().HasColumnName("ativo_id");
                entity.Property(e => e.Quantidade).IsRequired().HasColumnName("quantidade");
                entity.Property(e => e.PrecoMedio).IsRequired().HasPrecision(18, 4).HasColumnName("preco_medio");
                entity.Property(e => e.PL).IsRequired().HasPrecision(18, 4).HasColumnName("p_l");

                entity.HasIndex(e => new { e.IdUsuario, e.AtivoId }).HasDatabaseName("idx_usuario_ativo");
            });

            // Configuração da tabela Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nome).IsRequired().HasColumnName("nome");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.SenhaHash).IsRequired().HasColumnName("senha_hash");

            });

            // Configuração da tabela Ativo
            modelBuilder.Entity<Ativo>(entity =>
            {
                entity.ToTable("ativo");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nome).IsRequired().HasColumnName("nome");
                entity.Property(e => e.Tipo).HasColumnName("tipo");
            });
        }
    }
}
