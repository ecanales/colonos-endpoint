using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class LoginOut
    {
        public bool Success { get; set; }
        public int IdUsuario { get; set; }
        public string Error { get; set; }
        public string Usuario { get; set; }
    }
}
