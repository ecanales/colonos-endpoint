using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class VentaSocio
    {
        public Nullable<int> Year { get; set; }
        public string Mes { get; set; }
        public string SocioCode { get; set; }
        public Nullable<decimal> Total { get; set; }
    }
}
