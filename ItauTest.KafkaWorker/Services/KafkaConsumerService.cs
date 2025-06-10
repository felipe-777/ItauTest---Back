using Confluent.Kafka;
using ItauTest.Data;
using ItauTest.KafkaWorker.Models;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ItauTest.KafkaWorker.Services
{
    public class KafkaConsumerService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "cotacao-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("cotacoes");

            _logger.LogInformation("Iniciando consumo do tópico Kafka...");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(cancellationToken);

                    try
                    {
                        var message = JsonSerializer.Deserialize<CotacaoKafkaMessage>(result.Message.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
);


                        if (message == null || string.IsNullOrWhiteSpace(message.AtivoCodigo))
                        {
                            _logger.LogWarning("Mensagem Kafka inválida ou incompleta: {Mensagem}", result.Message.Value);
                            continue;
                        }

                        await ProcessarCotacaoAsync(message);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Erro ao desserializar mensagem Kafka: {Mensagem}", result.Message.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer cancelado.");
                consumer.Close();
            }
        }

        private async Task ProcessarCotacaoAsync(CotacaoKafkaMessage cotacaoMsg)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ItauDbContext>();

            var ativo = await db.Ativos.FirstOrDefaultAsync(a => a.Nome == cotacaoMsg.AtivoCodigo);
            if (ativo == null)
            {
                _logger.LogWarning($"Ativo '{cotacaoMsg.AtivoCodigo}' não encontrado.");
                return;
            }

            var novaCotacao = new Cotacao
            {
                AtivoId = ativo.Id,
                PrecoUnitario = cotacaoMsg.PrecoUnitario,
                DataHora = cotacaoMsg.DataHora
            };

            db.Cotacoes.Add(novaCotacao);
            await db.SaveChangesAsync();

            var posicoes = await db.Posicoes.Where(p => p.AtivoId == ativo.Id).ToListAsync();
            foreach (var posicao in posicoes)
            {
                posicao.PL = (cotacaoMsg.PrecoUnitario - posicao.PrecoMedio) * posicao.Quantidade;
            }

            await db.SaveChangesAsync();

            _logger.LogInformation($"Cotação processada: {cotacaoMsg.AtivoCodigo} R$ {cotacaoMsg.PrecoUnitario}");
        }
    }
}
