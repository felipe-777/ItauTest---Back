using ItauTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Interfaces.Repository
{
    public interface IOperacaoRepository
    {
        Task<IEnumerable<Operacao>> GetOperacoesByUsuarioAsync(long idUsuario, long? ativoId, DateTime? startDate = null);
        Task AddOperacaoAsync(Operacao operacao);
    }
}
