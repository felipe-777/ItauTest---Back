using ItauTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Interfaces.Repository
{
    public interface IPosicaoRepository
    {
        Task<Posicao> GetPosicaoAsync(long idUsuario, long ativoId);
        Task UpdatePosicaoAsync(Posicao posicao);
    }
}
