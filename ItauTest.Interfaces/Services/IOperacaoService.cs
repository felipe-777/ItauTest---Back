using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Interfaces.Services
{
    public interface IOperacaoService
    {
        Task AtualizarPosicaoAsync(int usuarioId, int ativoId);
    }
}
