using ItauTest.Data;
using ItauTest.KafkaWorker.Models;
using ItauTest.KafkaWorker.Services;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ItauTest.Tests.KafkaWorker.Services
{
    public class KafkaConsumerServiceTests
    {
        private readonly Mock<ILogger<KafkaConsumerService>> _loggerMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
        private readonly Mock<IServiceScope> _scopeMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ItauDbContext> _contextMock;

        private readonly KafkaConsumerService _consumerService;

        public KafkaConsumerServiceTests()
        {
            _loggerMock = new Mock<ILogger<KafkaConsumerService>>();
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();
            _scopeMock = new Mock<IServiceScope>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _contextMock = new Mock<ItauDbContext>();

            _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
            _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(ItauDbContext))).Returns(_contextMock.Object);

            _consumerService = new KafkaConsumerService(_loggerMock.Object, _scopeFactoryMock.Object);
        }

        [Fact]
        public async Task ProcessarCotacaoAsync_AdicionaCotacaoEAtualizaPL()
        {
            // Arrange
            var ativo = new Ativo { Id = 1, Nome = "PETR4" };
            var cotacaoMsg = new CotacaoKafkaMessage
            {
                AtivoCodigo = "PETR4",
                PrecoUnitario = 30.00m,
                DataHora = DateTime.UtcNow
            };
            var posicoes = new List<Posicao>
            {
                new Posicao { AtivoId = 1, Quantidade = 10, PrecoMedio = 25.00m }
            };

            _contextMock.Setup(x => x.Ativos)
                        .ReturnsDbSet(new List<Ativo> { ativo });

            _contextMock.Setup(x => x.Posicoes)
                        .ReturnsDbSet(posicoes);

            _contextMock.Setup(x => x.Cotacoes)
                        .ReturnsDbSet(new List<Cotacao>());

            // Act
            var method = typeof(KafkaConsumerService)
                                                     .GetMethod("ProcessarCotacaoAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

            var task = (Task)method.Invoke(_consumerService, new object[] { cotacaoMsg })!;
            await task;


            // Assert
            _contextMock.Verify(x => x.Cotacoes.Add(It.Is<Cotacao>(c =>
                c.AtivoId == ativo.Id &&
                c.PrecoUnitario == cotacaoMsg.PrecoUnitario
            )), Times.Once);

            Assert.Equal(50.00m, posicoes[0].PL); // (30 - 25) * 10

            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
