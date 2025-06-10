using ItauTest.Data;
using ItauTest.Data.Repositorys;
using ItauTest.Interfaces.Services;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class OperacaoRepositoryTests
{
    private ItauDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ItauDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ItauDbContext(options);
    }

    [Fact]
    public async Task AddOperacaoAsync_DeveAdicionarOperacaoEAtualizarPosicao()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var operacaoServiceMock = new Mock<IOperacaoService>();

        var repository = new OperacaoRepository(dbContext, operacaoServiceMock.Object);

        var operacao = new Operacao
        {
            IdUsuario = 1,
            AtivoId = 2,
            Quantidade = 10,
            PrecoUnitario = 100,
            Corretagem = 5,
            DataHora = DateTime.UtcNow
        };

        // Act
        await repository.AddOperacaoAsync(operacao);

        // Assert
        var operacoesSalvas = await dbContext.Operacoes.ToListAsync();
        Assert.Single(operacoesSalvas);
        Assert.Equal(operacao.IdUsuario, operacoesSalvas[0].IdUsuario);

        operacaoServiceMock.Verify(x =>
            x.AtualizarPosicaoAsync((int)operacao.IdUsuario, (int)operacao.AtivoId),
            Times.Once);
    }

    [Fact]
    public async Task GetOperacoesByUsuarioAsync_DeveRetornarOperacoesFiltradas()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var serviceMock = new Mock<IOperacaoService>();

        var operacoes = new List<Operacao>
        {
            new Operacao { IdUsuario = 1, AtivoId = 10, DataHora = DateTime.UtcNow.AddDays(-1), PrecoUnitario = 100 },
            new Operacao { IdUsuario = 1, AtivoId = 20, DataHora = DateTime.UtcNow, PrecoUnitario = 200 },
            new Operacao { IdUsuario = 2, AtivoId = 10, DataHora = DateTime.UtcNow, PrecoUnitario = 300 },
        };

        dbContext.Operacoes.AddRange(operacoes);
        await dbContext.SaveChangesAsync();

        var repository = new OperacaoRepository(dbContext, serviceMock.Object);

        // Act
        var result = await repository.GetOperacoesByUsuarioAsync(1, 20);

        // Assert
        Assert.Single(result);
        Assert.Equal(200, result.First().PrecoUnitario);
    }

    [Fact]
    public async Task GetOperacoesByUsuarioAsync_ComStartDate_DeveFiltrarPorData()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var serviceMock = new Mock<IOperacaoService>();

        var hoje = DateTime.UtcNow;

        dbContext.Operacoes.AddRange(
            new Operacao { IdUsuario = 1, AtivoId = 5, DataHora = hoje.AddDays(-3), PrecoUnitario = 50 },
            new Operacao { IdUsuario = 1, AtivoId = 5, DataHora = hoje, PrecoUnitario = 150 }
        );
        await dbContext.SaveChangesAsync();

        var repository = new OperacaoRepository(dbContext, serviceMock.Object);

        // Act
        var result = await repository.GetOperacoesByUsuarioAsync(1, 5, hoje.AddDays(-1));

        // Assert
        Assert.Single(result);
        Assert.Equal(150, result.First().PrecoUnitario);
    }
}
