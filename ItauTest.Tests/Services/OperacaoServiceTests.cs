using ItauTest.Data;
using ItauTest.Models;
using ItauTest.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ItauTest.Tests.Services
{
    public class OperacaoServiceTests
    {
        private readonly Mock<ItauDbContext> _contextMock;
        private readonly Mock<ILogger<OperacaoService>> _loggerMock;
        private readonly OperacaoService _operacaoService;

        public OperacaoServiceTests()
        {
            _contextMock = new Mock<ItauDbContext>();
            _loggerMock = new Mock<ILogger<OperacaoService>>();
            _operacaoService = new OperacaoService(_contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AtualizarPosicaoAsync_ComNovaPosicao_DeveAdicionarPosicao()
        {
            // Arrange
            var usuarioId = 1;
            var ativoId = 1;
            var operacoes = new List<Operacao>
            {
                new Operacao { IdUsuario = usuarioId, AtivoId = ativoId, Quantidade = 10, PrecoUnitario = 100 },
                new Operacao { IdUsuario = usuarioId, AtivoId = ativoId, Quantidade = 20, PrecoUnitario = 150 }
            };
            var cotacao = new Cotacao { AtivoId = ativoId, PrecoUnitario = 160, DataHora = DateTime.UtcNow };

            _contextMock.Setup(x => x.Operacoes).ReturnsDbSet(operacoes);
            _contextMock.Setup(x => x.Cotacoes).ReturnsDbSet(new List<Cotacao> { cotacao });
            _contextMock.Setup(x => x.Posicoes).ReturnsDbSet(new List<Posicao>());

            // Act
            await _operacaoService.AtualizarPosicaoAsync(usuarioId, ativoId);

            // Assert
            _contextMock.Verify(x => x.Posicoes.Add(It.Is<Posicao>(p =>
                p.IdUsuario == usuarioId &&
                p.AtivoId == ativoId &&
                p.Quantidade == 30 &&
                Math.Round(p.PrecoMedio, 3) == 133.333m &&
                Math.Round(p.PL, 3) == 800m
            )), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AtualizarPosicaoAsync_ComPosicaoExistente_DeveAtualizarPosicao()
        {
            // Arrange
            var usuarioId = 1;
            var ativoId = 1;
            var operacoes = new List<Operacao>
            {
                new Operacao { IdUsuario = usuarioId, AtivoId = ativoId, Quantidade = 10, PrecoUnitario = 100 },
                new Operacao { IdUsuario = usuarioId, AtivoId = ativoId, Quantidade = 20, PrecoUnitario = 150 }
            };
            var cotacao = new Cotacao { AtivoId = ativoId, PrecoUnitario = 160, DataHora = DateTime.UtcNow };
            var posicaoExistente = new Posicao { IdUsuario = usuarioId, AtivoId = ativoId, Quantidade = 0, PrecoMedio = 0, PL = 0 };

            _contextMock.Setup(x => x.Operacoes).ReturnsDbSet(operacoes);
            _contextMock.Setup(x => x.Cotacoes).ReturnsDbSet(new List<Cotacao> { cotacao });
            _contextMock.Setup(x => x.Posicoes).ReturnsDbSet(new List<Posicao> { posicaoExistente });

            // Act
            await _operacaoService.AtualizarPosicaoAsync(usuarioId, ativoId);

            // Assert
            Assert.Equal(30, posicaoExistente.Quantidade);
            Assert.Equal(133.333m, Math.Round(posicaoExistente.PrecoMedio, 3));
            Assert.Equal(800m, Math.Round(posicaoExistente.PL, 3));
            _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AtualizarPosicaoAsync_SemOperacoes_NaoAlteraNada()
        {
            // Arrange
            var usuarioId = 1;
            var ativoId = 1;
            _contextMock.Setup(x => x.Operacoes).ReturnsDbSet(new List<Operacao>());

            // Act
            await _operacaoService.AtualizarPosicaoAsync(usuarioId, ativoId);

            // Assert
            _contextMock.Verify(x => x.Posicoes.Add(It.IsAny<Posicao>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task AtualizarPosicaoAsync_SemCotacao_DeveCalcularPrecoMedioZerandoPL()
        {
            // Arrange
            var usuarioId = 1;
            var ativoId = 1;
            var operacoes = new List<Operacao>
            {
                new Operacao { IdUsuario = usuarioId, AtivoId = ativoId, Quantidade = 10, PrecoUnitario = 100 }
            };
            _contextMock.Setup(x => x.Operacoes).ReturnsDbSet(operacoes);
            _contextMock.Setup(x => x.Cotacoes).ReturnsDbSet(new List<Cotacao>());
            _contextMock.Setup(x => x.Posicoes).ReturnsDbSet(new List<Posicao>());

            // Act
            await _operacaoService.AtualizarPosicaoAsync(usuarioId, ativoId);

            // Assert
            _contextMock.Verify(x => x.Posicoes.Add(It.Is<Posicao>(p =>
                p.IdUsuario == usuarioId &&
                p.AtivoId == ativoId &&
                p.Quantidade == 10 &&
                p.PrecoMedio == 100m &&
                p.PL == 0
            )), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
