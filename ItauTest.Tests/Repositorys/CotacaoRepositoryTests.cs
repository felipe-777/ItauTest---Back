using ItauTest.Data;
using ItauTest.Data.Repositorys;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CotacaoRepositoryTests
{
    private ItauDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ItauDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Garante DB limpo por teste
            .Options;

        return new ItauDbContext(options);
    }

    [Fact]
    public async Task AddCotacaoAsync_DeveSalvarCotacao()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new CotacaoRepository(context);

        var cotacao = new Cotacao
        {
            AtivoId = 1,
            PrecoUnitario = 10.5m,
            DataHora = DateTime.UtcNow
        };

        // Act
        await repository.AddCotacaoAsync(cotacao);

        // Assert
        var cotacoes = await context.Cotacoes.ToListAsync();
        Assert.Single(cotacoes);
        Assert.Equal(10.5m, cotacoes.First().PrecoUnitario);
    }

    [Fact]
    public async Task GetLatestCotacaoAsync_DeveRetornarMaisRecente()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        context.Cotacoes.AddRange(
            new Cotacao { AtivoId = 1, PrecoUnitario = 10, DataHora = DateTime.UtcNow.AddMinutes(-10) },
            new Cotacao { AtivoId = 1, PrecoUnitario = 20, DataHora = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var repository = new CotacaoRepository(context);

        // Act
        var result = await repository.GetLatestCotacaoAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.PrecoUnitario);
    }

    [Fact]
    public async Task GetCotacoesByAtivoAsync_DeveFiltrarPorDatas()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var agora = DateTime.UtcNow;

        context.Cotacoes.AddRange(
            new Cotacao { AtivoId = 1, PrecoUnitario = 10, DataHora = agora.AddDays(-3) },
            new Cotacao { AtivoId = 1, PrecoUnitario = 20, DataHora = agora.AddDays(-2) },
            new Cotacao { AtivoId = 1, PrecoUnitario = 30, DataHora = agora.AddDays(-1) }
        );
        await context.SaveChangesAsync();

        var repository = new CotacaoRepository(context);

        // Act
        var resultados = await repository.GetCotacoesByAtivoAsync(1, agora.AddDays(-2.5), agora.AddDays(-1.5));

        // Assert
        Assert.Single(resultados);
        Assert.Equal(20, resultados.First().PrecoUnitario);
    }
}
