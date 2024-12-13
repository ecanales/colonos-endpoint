using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class MensajeReturn
    {
        public HttpStatusCode statuscode { get; set; }
        public int count { get; set; }
        public bool error { get; set; }
        public string msg { get; set; }
        public dynamic data { get; set; }

    }
}
