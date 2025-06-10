using ItauTest.Interfaces.Repository;
using ItauTest.Interfaces.Services;
using ItauTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Services.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly IOperacaoRepository _operacaoRepository;
        private readonly ICotacaoRepository _cotacaoRepository;
        private readonly IPosicaoRepository _posicaoRepository;

        public InvestmentService(IOperacaoRepository operacaoRepository, ICotacaoRepository cotacaoRepository, IPosicaoRepository posicaoRepository)
        {
            _operacaoRepository = operacaoRepository;
            _cotacaoRepository = cotacaoRepository;
            _posicaoRepository = posicaoRepository; 
        }

        public async Task<decimal> CalculateAveragePriceAsync(long idUsuario, long ativoId)
        {
            var operacoes = await _operacaoRepository.GetOperacoesByUsuarioAsync(idUsuario, ativoId);
            if (!operacoes.Any())
            {
                throw new InvalidOperationException("Nenhuma operação encontrada para o usuário e ativo especificados.");
            }

            decimal totalCost = 0;
            int totalQuantity = 0;

            foreach (var operacao in operacoes)
            {
                if (operacao.Quantidade <= 0)
                {
                    throw new ArgumentException("A quantidade deve ser maior que zero.");
                }
                totalCost += operacao.Quantidade * operacao.PrecoUnitario;
                totalQuantity += operacao.Quantidade;
            }

            if (totalQuantity == 0)
            {
                throw new InvalidOperationException("A quantidade total não pode ser zero.");
            }

            return totalCost / totalQuantity;
        }

        public async Task<decimal> CalculateTotalBrokerageFeesAsync(long idUsuario)
        {
            var operacoes = await _operacaoRepository.
                GetOperacoesByUsuarioAsync(idUsuario, null, null);
            return operacoes.Sum(o => o.Corretagem);
        }

        public async Task<Posicao> CalculatePositionAsync(long idUsuario, long ativoId)
        {
            var operacoes = await _operacaoRepository.
                GetOperacoesByUsuarioAsync(idUsuario, ativoId);
            var cotacao = await _cotacaoRepository.
                GetLatestCotacaoAsync(idUsuario);
            var posicao = await _posicaoRepository.
                GetPosicaoAsync(idUsuario, ativoId);

            if (operacoes == null || !operacoes.Any())
            {
                throw new InvalidOperationException("Nenhuma operação encontrada para o usuário e ativo especificados.");
            }

            int totalQuantity = operacoes.Sum(o => o.Quantidade);
            decimal totalCost = operacoes.Sum(o => o.Quantidade * o.PrecoUnitario);
            decimal averagePrice = totalCost / totalQuantity;

            decimal pl = cotacao != null ? (cotacao.PrecoUnitario - averagePrice) * totalQuantity : 0;

            if (posicao == null)
            {
                posicao = new Posicao
                {
                    IdUsuario = idUsuario,
                    AtivoId = ativoId,
                    Quantidade = totalQuantity,
                    PrecoMedio = averagePrice,
                    PL = pl
                };
                await _posicaoRepository.UpdatePosicaoAsync(posicao);
            }
            else
            {
                posicao.Quantidade = totalQuantity;
                posicao.PrecoMedio = averagePrice;
                posicao.PL = pl;
                await _posicaoRepository.UpdatePosicaoAsync(posicao);
            }

            return posicao;

        }
        public async Task<IEnumerable<Cotacao>> GetHistoricalPricesAsync(long ativoId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _cotacaoRepository.GetCotacoesByAtivoAsync(ativoId, startDate, endDate);
        }
    }
}

