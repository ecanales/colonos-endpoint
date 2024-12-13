using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Drivin
{
    public class ClienteRequest
    {
        
        public string code {get;set;}
        public string address {get;set;}
        public string reference {get;set;}
        public string city {get;set;}
        public string county {get;set;}
        public string country {get;set;}
        public decimal lat {get;set;}
        public decimal lng {get;set;}
        public string client_name {get;set;}
        public string contact_name {get;set;}
        public string contact_email {get;set;}
        public List<Time_windows> time_windows { get; set; }
        public List<OrderRequest> orders { get; set; }
    }
}
