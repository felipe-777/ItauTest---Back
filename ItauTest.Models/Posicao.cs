namespace ItauTest.Models
{
    public class Posicao
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long AtivoId { get; set; } 
        public int Quantidade { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal PL { get; set; }
    }
}
