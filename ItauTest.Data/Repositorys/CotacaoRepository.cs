using ItauTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItauTest.Interfaces;
using ItauTest.Interfaces.Repository;

namespace ItauTest.Data.Repositorys
{
    public class CotacaoRepository : ICotacaoRepository
    {
        private readonly ItauDbContext _context;

        public CotacaoRepository(ItauDbContext context)
        {
            _context = context;
        }

        public async Task<Cotacao> GetLatestCotacaoAsync(long ativoId)
        {
            return await _context.Cotacoes
                .Where(c => c.AtivoId == ativoId)
                .OrderByDescending(c => c.DataHora)
                .FirstOrDefaultAsync();
        }

        public async Task AddCotacaoAsync(Cotacao cotacao)
        {
            await _context.Cotacoes.AddAsync(cotacao);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cotacao>> GetCotacoesByAtivoAsync(long ativoId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Cotacoes.Where(c => c.AtivoId == ativoId);

            if (startDate.HasValue)
            {
                query = query.Where(c => c.DataHora >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.DataHora <= endDate.Value);
            }

            return await query.OrderBy(c => c.DataHora).ToListAsync();
        }
    }
}
