using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class HistorialModificaciones
    {
        public int IdHistorial { get; set; }
        public string SocioCode { get; set; }
        public string RazonSocial { get; set; }
        public Nullable<System.DateTime> FechaProceso { get; set; }
        public string Cambios { get; set; }
        public string Usuario { get; set; }
    }
}
