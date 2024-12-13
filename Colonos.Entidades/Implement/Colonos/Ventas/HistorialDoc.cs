using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class HistorialDoc
    {
        public Nullable<int> Numero { get; set; }
        public string Estado { get; set; }
        public Nullable<int> IdTipo { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
        public string Tipo { get; set; }
    }
}
