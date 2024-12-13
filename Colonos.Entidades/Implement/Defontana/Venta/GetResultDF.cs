using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class GetResultDF: SaveSaleReturn
    {
        
        public int totalItems {get;set;}
        public int pageNumber {get;set;}
        public int itemsPerPage {get;set;}
        public List<ClienteDF> clientList { get; set; }
    }
}
