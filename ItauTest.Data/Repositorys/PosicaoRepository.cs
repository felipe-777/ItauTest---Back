
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
    public class PosicaoRepository : IPosicaoRepository
    {
        private readonly ItauDbContext _context;

        public PosicaoRepository(ItauDbContext context)
        {
            _context = context;
        }

        public async Task<Posicao> GetPosicaoAsync(long idUsuario, long ativoId)
        {
            return await _context.Posicoes
                .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.AtivoId == ativoId);
        }

        public async Task UpdatePosicaoAsync(Posicao posicao)
        {
            _context.Posicoes.Update(posicao);
            await _context.SaveChangesAsync();
        }
    }
}
