using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class SaveSaleReturn
    {
        public string documentType {get;set;}
        public int firstFolio {get;set;}
        public int lastFolio {get;set;}
        public bool success {get;set;}
        public string message {get;set;}
        public int number { get;set;}
        public string exceptionMessage { get;set;}
    }
}
