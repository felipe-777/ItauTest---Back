namespace ItauTest.Models
{
    public class Operacao
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long AtivoId { get; set; } 
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Corretagem { get; set; }
    }
}
