using ItauTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Interfaces.Repository
{
    public interface ICotacaoRepository
    {
        Task<Cotacao> GetLatestCotacaoAsync(long ativoId);
        Task AddCotacaoAsync(Cotacao cotacao);
        Task<IEnumerable<Cotacao>> GetCotacoesByAtivoAsync(long ativoId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
