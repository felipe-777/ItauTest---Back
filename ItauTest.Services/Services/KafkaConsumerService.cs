using ItauTest.Data;
using ItauTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace ItauTest.Services.Services
{
    public class KafkaConsumerService
    {
        private readonly ItauDbContext _context;
        private readonly string _bootstrapServers;
        private readonly string _groupId;
        private readonly string _topic;

        public KafkaConsumerService(ItauDbContext context, string bootstrapServers, string groupId, string topic)
        {
            _context = context;
            _bootstrapServers = bootstrapServers;
            _groupId = groupId;
            _topic = topic;
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        if (consumeResult != null)
                        {
                            var message = consumeResult.Message.Value;
                            var cotacao = JsonSerializer.Deserialize<Cotacao>(message);
                            if (cotacao != null)
                            {
                                _context.Cotacoes.Add(cotacao);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
