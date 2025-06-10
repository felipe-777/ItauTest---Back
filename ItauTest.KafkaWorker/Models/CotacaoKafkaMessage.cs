using System;
using System.Text.Json.Serialization;

namespace ItauTest.KafkaWorker.Models
{
    public class CotacaoKafkaMessage
    {
        [JsonPropertyName("ativoCodigo")]
        public string AtivoCodigo { get; set; }

        [JsonPropertyName("precoUnitario")]
        public decimal PrecoUnitario { get; set; }

        [JsonPropertyName("dataHora")]
        public DateTime DataHora { get; set; }
    }
}
