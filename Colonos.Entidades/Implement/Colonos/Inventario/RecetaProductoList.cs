using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class RecetaProductoList
    {
        
        public string ProdCode {get;set;}
	    public string ProdCodeRef {get;set;}
        public string ProdNombreRef { get; set; }
        public decimal Cantidad {get;set;}
        public string TipoCode { get; set; }
        public decimal? Costo { get; set; }
        public decimal? Stock { get; set; }
        public string BodegaCode { get; set; }

    }
}
