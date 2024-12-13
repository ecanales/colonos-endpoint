using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Drivin
{
    public class OrderRequest
    {
        public string code { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string supplier_name { get; set; }
        public List<ItemRequest> items { get; set; }
    }
}
