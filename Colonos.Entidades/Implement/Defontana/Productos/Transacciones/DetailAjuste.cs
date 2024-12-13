using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class DetailAjuste
    {
        
        public string articleId {get;set;}
        public string description {get;set;}
        public decimal count {get;set;}
        public string coinId {get;set;}
        public string comment {get;set;}
        public decimal price {get;set;}
        /*
        public string tipoCentDet {get;set;}
        public string serialPrefix {get;set;}
        public string serialSufix {get;set;}
        public string serialStart {get;set;}
        public List<string> serials {get;set;}
        public List<string> lotes {get;set;}
        */
        public AnalisysAjuste analysis {get;set;} 
        
    }
}
