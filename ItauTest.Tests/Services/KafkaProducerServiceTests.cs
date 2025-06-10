using Confluent.Kafka;
using ItauTest.KafkaWorker.Models;
using ItauTest.KafkaWorker.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;
using System.Threading.Tasks;

public class KafkaProducerServiceTests
{
    [Fact]
    public async Task PublicarCotacaoAsync_DeveProduzirMensagemNoKafka()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<KafkaProducerService>>();
        var producerMock = new Mock<IProducer<Null, string>>();

        var cotacao = new CotacaoKafkaMessage
        {
            AtivoCodigo = "TESTE",
            PrecoUnitario = 123.45m,
            DataHora = DateTime.UtcNow
        };

        var expectedJson = JsonSerializer.Serialize(cotacao);
        var expectedMessage = new Message<Null, string> { Value = expectedJson };

        // Simula retorno do Kafka
        producerMock
            .Setup(p => p.ProduceAsync("cotacoes",
                It.Is<Message<Null, string>>(m => m.Value == expectedJson),
                default))
            .ReturnsAsync(new DeliveryResult<Null, string>
            {
                Partition = new Partition(1),
                Message = expectedMessage
            });

        // Usa o construtor alternativo com injeção do mock
        var service = new KafkaProducerService(loggerMock.Object, producerMock.Object);

        // Act
        await service.PublicarCotacaoAsync(cotacao);

        // Assert
        producerMock.Verify(p => p.ProduceAsync("cotacoes",
            It.Is<Message<Null, string>>(m => m.Value == expectedJson),
            default), Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Mensagem publicada no Kafka")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
