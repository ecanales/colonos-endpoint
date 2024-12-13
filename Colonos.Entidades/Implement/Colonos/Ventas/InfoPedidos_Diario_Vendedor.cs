using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class InfoPedidos_Diario_Vendedor
    {
        public string VendedorNombre { get; set; }
        public Nullable<int> Pedidos { get; set; }
        public Nullable<decimal> Kilos { get; set; }
        public Nullable<decimal> KilosPromedio { get; set; }
        public Nullable<decimal> PedidosporHora { get; set; }
    }
}
