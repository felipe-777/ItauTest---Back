using ItauTest.Data;
using ItauTest.Interfaces.Repository;
using ItauTest.Models;
using ItauTest.Services;
using ItauTest.Services.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ItauTest.Tests.Services
{
    public class InvestmentServiceTests
    {
        private readonly Mock<IOperacaoRepository> _operacaoRepositoryMock;
        private readonly Mock<ICotacaoRepository> _cotacaoRepositoryMock;
        private readonly Mock<IPosicaoRepository> _posicaoRepositoryMock;
        private readonly InvestmentService _investmentService;

        public InvestmentServiceTests()
        {
            _operacaoRepositoryMock = new Mock<IOperacaoRepository>();
            _cotacaoRepositoryMock = new Mock<ICotacaoRepository>();
            _posicaoRepositoryMock = new Mock<IPosicaoRepository>();
            _investmentService = new InvestmentService(
                _operacaoRepositoryMock.Object,
                _cotacaoRepositoryMock.Object,
                _posicaoRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CalculateAveragePriceAsync_ValidInput_ReturnsCorrectAverage()
        {
            // Arrange
            var operacoes = new List<Operacao>
            {
                new Operacao { IdUsuario = 1, AtivoId = 1, Quantidade = 10, PrecoUnitario = 100 },
                new Operacao { IdUsuario = 1, AtivoId = 1, Quantidade = 20, PrecoUnitario = 150 }
            };
            _operacaoRepositoryMock.Setup(repo => repo.GetOperacoesByUsuarioAsync(1, 1, null))
                .ReturnsAsync(operacoes);

            // Act
            var result = await _investmentService.CalculateAveragePriceAsync(1, 1);

            // Assert
            Assert.Equal(133.333m, result, 3);
        }

        [Fact]
        public async Task CalculateAveragePriceAsync_EmptyList_ThrowsInvalidOperationException()
        {
            // Arrange
            _operacaoRepositoryMock.Setup(repo => repo.GetOperacoesByUsuarioAsync(1, 1, null))
                .ReturnsAsync(new List<Operacao>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _investmentService.CalculateAveragePriceAsync(1, 1));
        }

        [Fact]
        public async Task CalculateTotalBrokerageFeesAsync_ValidInput_ReturnsCorrectTotal()
        {
            // Arrange
            var operacoes = new List<Operacao>
            {
                new Operacao { IdUsuario = 1, AtivoId = 1, Corretagem = 10 },
                new Operacao { IdUsuario = 1, AtivoId = 2, Corretagem = 15 }
            };
            _operacaoRepositoryMock.Setup(repo => repo.GetOperacoesByUsuarioAsync(1, null, null))
                .ReturnsAsync(operacoes);

            // Act
            var result = await _investmentService.CalculateTotalBrokerageFeesAsync(1);

            // Assert
            Assert.Equal(25m, result);
        }

        [Fact]
        public async Task CalculatePositionAsync_ValidInput_ReturnsCorrectPosition()
        {
            // Arrange
            var operacoes = new List<Operacao>
            {
                new Operacao { IdUsuario = 1, AtivoId = 1, Quantidade = 10, PrecoUnitario = 100 },
                new Operacao { IdUsuario = 1, AtivoId = 1, Quantidade = 20, PrecoUnitario = 150 }
            };
            var cotacao = new Cotacao { AtivoId = 1, PrecoUnitario = 160, DataHora = DateTime.Now };
            var posicao = new Posicao { IdUsuario = 1, AtivoId = 1, Quantidade = 0, PrecoMedio = 0, PL = 0 };

            _operacaoRepositoryMock.Setup(repo => repo.GetOperacoesByUsuarioAsync(1, 1, null))
                .ReturnsAsync(operacoes);
            _cotacaoRepositoryMock.Setup(repo => repo.GetLatestCotacaoAsync(1))
                .ReturnsAsync(cotacao);
            _posicaoRepositoryMock.Setup(repo => repo.GetPosicaoAsync(1, 1))
                .ReturnsAsync(posicao);
            _posicaoRepositoryMock.Setup(repo => repo.UpdatePosicaoAsync(It.IsAny<Posicao>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _investmentService.CalculatePositionAsync(1, 1);

            // Assert
            Assert.Equal(30, result.Quantidade);
            Assert.Equal(133.333m, result.PrecoMedio, 3);
            Assert.Equal(800m, result.PL, 3); // (160 - 133.333) * 30
        }

        [Fact]
        public async Task GetHistoricalPricesAsync_ValidInput_ReturnsCorrectPrices()
        {
            // Arrange
            var cotações = new List<Cotacao>
            {
                new Cotacao { AtivoId = 1, PrecoUnitario = 100, DataHora = DateTime.Now.AddDays(-1) },
                new Cotacao { AtivoId = 1, PrecoUnitario = 105, DataHora = DateTime.Now }
            };
            _cotacaoRepositoryMock.Setup(repo => repo.GetCotacoesByAtivoAsync(1, null, null))
                .ReturnsAsync(cotações);

            // Act
            var result = await _investmentService.GetHistoricalPricesAsync(1);

            // Assert
            Assert.Equal(cotações, result);
        }
    }
}