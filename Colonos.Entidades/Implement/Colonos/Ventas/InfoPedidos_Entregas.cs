using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class InfoPedidos_Entregas
    {
        public string Bodega { get; set; }
        public string RangoKilos { get; set; }
        public Nullable<int> Ayer { get; set; }
        public Nullable<int> Hoy { get; set; }
        public Nullable<int> Mañana { get; set; }
        public Nullable<int> Pedidos { get; set; }
    }
}
