using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class Details
    {
        
        public string type {get;set;}
        public bool isExempt {get;set;}
        public string code {get;set;}
        public decimal count {get;set;}
        public string productName {get;set;}
        public string productNameBarCode {get;set;}
        public string comment {get;set;}
        public decimal price {get;set;}
        public Discount discount { get; set; }
        public string unit { get; set; }
        public Analysis analysis { get; set; }
        public bool useBatch { get; set; }
        public List<string> batchInfo { get; set; }
    }
}
