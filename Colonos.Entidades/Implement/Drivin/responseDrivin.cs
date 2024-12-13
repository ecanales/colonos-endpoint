using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Drivin
{
    public class responseDrivin
    {
        
        public string scenario_token {get;set;}
        public int addresses_count {get;set;}
        public int orders_count {get;set;}
        public int items_count {get;set;}
        public int vehicles_count {get;set;}
        public string description { get; set;}
    }
}
