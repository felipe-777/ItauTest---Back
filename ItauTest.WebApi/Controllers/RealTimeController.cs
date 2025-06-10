using ItauTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItauTest.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeController : ControllerBase
    {
        [HttpGet("updates")]
        public async Task GetUpdates()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var cotacao = await GetLatestCotacao(); 
                await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(cotacao)}\n\n");
                await Response.Body.FlushAsync();
                await Task.Delay(5000); 
            }
        }

        private async Task<Cotacao> GetLatestCotacao()
        {
            // Simulação: obter a última cotação do banco (substituir por lógica real)
            return new Cotacao { AtivoId = 1, PrecoUnitario = 150.0m, DataHora = DateTime.UtcNow };
        }
    }
}
