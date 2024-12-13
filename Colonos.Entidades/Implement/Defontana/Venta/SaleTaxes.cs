using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class SaleTaxes
    {
        public string code { get; set; }
        public decimal value { get; set; }
        public Analysis taxeAnalysis { get; set; }
    }
}
