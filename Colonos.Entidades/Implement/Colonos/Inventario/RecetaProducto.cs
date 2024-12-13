using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class RecetaProducto
    {
        public string ProdCode { get; set; }
        public string ProdNombre { get; set; }
        public List<RecetaProductoList> Lineas { get; set; }
    }
}
