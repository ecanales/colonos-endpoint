using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class productListDF
    {
        public bool success { get; set; }
        public int totalItems { get; set; }
        public List<ProductosDF> productList { get; set; }
    }
}
