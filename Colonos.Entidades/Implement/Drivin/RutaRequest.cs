using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Drivin
{
    public class RutaRequest
    {
        public string vehicle_code { get; set; }
        public List<ClienteRequest> clients { get; set; }
    }
}
