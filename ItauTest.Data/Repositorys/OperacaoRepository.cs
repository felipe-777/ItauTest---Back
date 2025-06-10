using ItauTest.Interfaces.Services;
using ItauTest.Interfaces.Repository;
using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ItauTest.Data.Repositorys
{
    public class OperacaoRepository : IOperacaoRepository
    {
        private readonly ItauDbContext _context;
        private readonly IOperacaoService _operacaoService;

        public OperacaoRepository(ItauDbContext context, IOperacaoService operacaoService)
        {
            _context = context;
            _operacaoService = operacaoService;
        }

        public async Task<IEnumerable<Operacao>> GetOperacoesByUsuarioAsync(long idUsuario, long? ativoId, DateTime? startDate = null)
        {
            var query = _context.Operacoes
                .Where(o => o.IdUsuario == idUsuario);

            if (ativoId.HasValue)
            {
                query = query.Where(o => o.AtivoId == ativoId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.DataHora >= startDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task AddOperacaoAsync(Operacao operacao)
        {
            await _context.Operacoes.AddAsync(operacao);
            await _context.SaveChangesAsync();
            await _operacaoService.AtualizarPosicaoAsync((int)operacao.IdUsuario, (int)operacao.AtivoId);
        }
    }
}
