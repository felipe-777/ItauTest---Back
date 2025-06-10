using ItauTest.Data;
using ItauTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using ItauTest.Interfaces.Services;
using ItauTest.Interfaces.Repository;

namespace ItauTest.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OperacaoController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;
        private readonly IOperacaoRepository _operacaoRepository;
        private readonly ItauDbContext _context;

        public OperacaoController(IInvestmentService investmentService, IOperacaoRepository operacaoRepository, ItauDbContext context)
        {
            _investmentService = investmentService;
            _operacaoRepository = operacaoRepository;
            _context = context;
        }

        [HttpGet("average-price/{idUsuario}/{ativoId}")]
        public async Task<ActionResult<decimal>> GetAveragePrice(long idUsuario, long ativoId)
        {
            try
            {
                var averagePrice = await _investmentService.CalculateAveragePriceAsync(idUsuario, ativoId);
                return Ok(averagePrice);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Operacao>>> GetOperacoes(long idUsuario, long ativoId)
        {
            try
            {
                var operacoes = await _operacaoRepository.
                    GetOperacoesByUsuarioAsync(idUsuario, ativoId);
                return Ok(operacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar operações: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddOperacao([FromBody] Operacao operacao)
        {
            try
            {
                await _operacaoRepository.AddOperacaoAsync(operacao);
                return CreatedAtAction(nameof(GetOperacoes), new { idUsuario = operacao.IdUsuario, ativoId = operacao.AtivoId }, operacao);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao adicionar operação: {ex.Message}");
            }
        }

        [HttpGet("brokerage-fees/{idUsuario}")]
        public async Task<ActionResult<decimal>> GetTotalBrokerageFees(long idUsuario)
        {
            try
            {
                var totalFees = await _investmentService.CalculateTotalBrokerageFeesAsync(idUsuario);
                return Ok(totalFees.ToString("F2", CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao calcular taxas: {ex.Message}");
            }
        }

        [HttpGet("position/{idUsuario}/{ativoId}")]
        public async Task<ActionResult<Posicao>> GetPosition(long idUsuario, long ativoId)
        {
            try
            {
                var posicao = await _investmentService.CalculatePositionAsync(idUsuario, ativoId);
                return Ok(posicao);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao calcular posição: {ex.Message}");
            }
        }

        [HttpGet("usuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<Ativo>>> GetAtivos()
        {
            var ativos = await _context.Ativos.ToListAsync();
            return Ok(ativos);
        }

        [HttpPost("usuarios")]
        public async Task<ActionResult<Usuario>> AddUsuario([FromBody] Usuario usuarioDto)
        {
            if (string.IsNullOrWhiteSpace(usuarioDto.Nome) || string.IsNullOrWhiteSpace(usuarioDto.Email))
                return BadRequest("Nome e e-mail são obrigatórios.");

            if (string.IsNullOrWhiteSpace(usuarioDto.SenhaHash))
                return BadRequest("A senha é obrigatória.");

            bool emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email);
            if (emailJaExiste)
                return Conflict("Já existe um usuário com esse e-mail.");

            var hasher = new PasswordHasher<Usuario>();
            var usuario = new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email
            };
            usuario.SenhaHash = hasher.HashPassword(usuario, usuarioDto.SenhaHash);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);
        }

        [HttpGet("historical-prices/{ativoId}")]
        public async Task<ActionResult<IEnumerable<Cotacao>>> GetHistoricalPrices(long ativoId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var prices = await _investmentService.GetHistoricalPricesAsync(ativoId, startDate, endDate);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar preços históricos: {ex.Message}");
            }
        }
    }
}
