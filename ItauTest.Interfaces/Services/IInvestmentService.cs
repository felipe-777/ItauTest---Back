using ItauTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Interfaces.Services
{
    public interface IInvestmentService
    {
        Task<decimal> CalculateAveragePriceAsync(long idUsuario, long ativoId);
        Task<decimal> CalculateTotalBrokerageFeesAsync(long idUsuario);
        Task<Posicao> CalculatePositionAsync(long idUsuario, long ativoId);
        Task<IEnumerable<Cotacao>> GetHistoricalPricesAsync(long ativoId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
