using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class ProductosDF_stockDetail
    {
        
        public string companyID {get;set;}
		public string productID {get;set;}
		public string storageID {get;set;}
		public decimal stock {get;set;}
		public decimal totalReservedStock {get;set;}
        
    }
}
