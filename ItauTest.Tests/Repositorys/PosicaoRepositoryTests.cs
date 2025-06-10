using ItauTest.Data;
using ItauTest.Data.Repositorys;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

public class PosicaoRepositoryTests
{
    private ItauDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ItauDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ItauDbContext(options);
    }

    [Fact]
    public async Task GetPosicaoAsync_DeveRetornarPosicaoExistente()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();

        var posicao = new Posicao
        {
            IdUsuario = 1,
            AtivoId = 2,
            Quantidade = 100,
            PrecoMedio = 50,
            PL = 500
        };

        dbContext.Posicoes.Add(posicao);
        await dbContext.SaveChangesAsync();

        var repository = new PosicaoRepository(dbContext);

        // Act
        var result = await repository.GetPosicaoAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.Quantidade);
        Assert.Equal(50, result.PrecoMedio);
    }

    [Fact]
    public async Task GetPosicaoAsync_DeveRetornarNull_SeNaoEncontrar()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var repository = new PosicaoRepository(dbContext);

        // Act
        var result = await repository.GetPosicaoAsync(999, 888);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePosicaoAsync_DeveAtualizarPosicao()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();

        var posicao = new Posicao
        {
            IdUsuario = 1,
            AtivoId = 2,
            Quantidade = 10,
            PrecoMedio = 20,
            PL = 100
        };

        dbContext.Posicoes.Add(posicao);
        await dbContext.SaveChangesAsync();

        var repository = new PosicaoRepository(dbContext);

        // Act
        posicao.Quantidade = 50;
        posicao.PrecoMedio = 30;
        await repository.UpdatePosicaoAsync(posicao);

        // Assert
        var updated = await dbContext.Posicoes.FirstAsync();
        Assert.Equal(50, updated.Quantidade);
        Assert.Equal(30, updated.PrecoMedio);
    }
}
