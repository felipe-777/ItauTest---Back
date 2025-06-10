using Confluent.Kafka;
using ItauTest.KafkaWorker.Models;
using ItauTest.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ItauTest.KafkaWorker.Services
{
    public class KafkaProducerService
    {
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(ILogger<KafkaProducerService> logger, IProducer<Null, string> producer)
        {
            _logger = logger;
            _producer = producer;
        }

        // Construtor padrão usado no app real
        public KafkaProducerService(ILogger<KafkaProducerService> logger)
            : this(logger, new ProducerBuilder<Null, string>(
                  new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
        {
        }

        public async Task PublicarCotacaoAsync(CotacaoKafkaMessage cotacao)
        {
            var json = JsonSerializer.Serialize(cotacao);
            var message = new Message<Null, string> { Value = json };

            var result = await _producer.ProduceAsync("cotacoes", message);
            _logger.LogInformation($"Mensagem publicada no Kafka: {json} (Partição: {result.Partition})");
        }
    }

}
