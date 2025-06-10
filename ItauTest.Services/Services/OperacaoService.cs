using ItauTest.Data;
using ItauTest.Interfaces.Services;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Services.Services
{
    public class OperacaoService : IOperacaoService
    {
        private readonly ItauDbContext _db;
        private readonly ILogger<OperacaoService> _logger;

        public OperacaoService(ItauDbContext db, ILogger<OperacaoService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task AtualizarPosicaoAsync(int usuarioId, int ativoId)
        {
            var compras = await _db.Operacoes
                .Where(o => o.IdUsuario == usuarioId && o.AtivoId == ativoId)
                .ToListAsync();

            if (!compras.Any())
                return;

            var totalQuantidade = compras.Sum(o => o.Quantidade);
            var totalInvestido = compras.Sum(o => o.Quantidade * o.PrecoUnitario);
            var precoMedio = totalInvestido / totalQuantidade;

            var posicao = await _db.Posicoes
                .FirstOrDefaultAsync(p => p.IdUsuario == usuarioId && p.AtivoId == ativoId);

            if (posicao == null)
            {
                posicao = new Posicao
                {
                    IdUsuario = usuarioId,
                    AtivoId = ativoId
                };
                _db.Posicoes.Add(posicao);
            }

            posicao.Quantidade = totalQuantidade;
            posicao.PrecoMedio = precoMedio;

            var ultimaCotacao = await _db.Cotacoes
                .Where(c => c.AtivoId == ativoId)
                .OrderByDescending(c => c.DataHora)
                .FirstOrDefaultAsync();

            if (ultimaCotacao != null)
            {
                posicao.PL = (ultimaCotacao.PrecoUnitario - posicao.PrecoMedio) * posicao.Quantidade;
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation($"Posição atualizada: Usuário {usuarioId}, Ativo {ativoId}, Qtd {totalQuantidade}, PM R$ {precoMedio}");
        }
    }
}
