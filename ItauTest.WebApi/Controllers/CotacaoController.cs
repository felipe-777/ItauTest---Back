using ItauTest.KafkaWorker.Models;
using ItauTest.KafkaWorker.Services;
using ItauTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItauTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotacaoController : ControllerBase
    {
        private readonly KafkaProducerService _kafkaProducerService;

        public CotacaoController(KafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }

        [HttpPost("publicar")]
        public async Task<IActionResult> PublicarCotacao([FromBody] CotacaoKafkaMessage cotacao)
        {
            if (string.IsNullOrWhiteSpace(cotacao.AtivoCodigo) || cotacao.PrecoUnitario <= 0)
                return BadRequest("Dados da cotação inválidos.");

            await _kafkaProducerService.PublicarCotacaoAsync(cotacao);
            return Ok("Cotação publicada com sucesso.");
        }
    }
}
