using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItauTest.Models
{
    public class Ativo
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string? Tipo { get; set; } // Opcional: Ação, FII, ETF etc.
    }
}
