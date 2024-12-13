using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class ItemBandeja
    {
        public int DocEntry { get; set; }
        public string BandejaCode { get; set; }
        public Nullable<int> DocLinea { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        public string UsuarioCodeIngreso { get; set; }
        public string MotivoIngreso { get; set; }
        public Nullable<System.DateTime> FechaAutorizado { get; set; }
        public Nullable<bool> Autorizado { get; set; }
        public string UsuarioCodeAutoriza { get; set; }
        public string MotivoRechazo { get; set; }
        public Nullable<int> LineaItem { get; set; }
        public Nullable<decimal> ValorSolicitado { get; set; }
        public Nullable<decimal> ValorRegla { get; set; }
        public Nullable<decimal> MargenRegla { get; set; }
    }

}
