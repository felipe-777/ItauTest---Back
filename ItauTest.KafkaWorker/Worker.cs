using ItauTest.KafkaWorker.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ItauTest.KafkaWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly KafkaConsumerService _consumerService;

        public Worker(ILogger<Worker> logger, KafkaConsumerService consumerService)
        {
            _logger = logger;
            _consumerService = consumerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando o Worker KafkaConsumer...");
            await _consumerService.StartConsumingAsync(stoppingToken);
        }
    }
}
