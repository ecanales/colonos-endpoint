using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class TokenResult
    {
        
        public bool success {get;set;}
        public string message {get;set;}
        public string access_token {get;set;}
        public int expires_in {get;set;}
        public string token_type {get;set;}
        
    }
}
