using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class TransaccionLinea
    {
        public int DocEntry { get; set; }
        public int DocLinea { get; set; }
        public string ProdCode { get; set; }
        public string ProdNombre { get; set; }
        public decimal CantidadSolicitada { get; set; }
        public decimal Costo { get; set; }
        public string MedidaCode { get; set; }
    }
}
