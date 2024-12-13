using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades.Defontana
{
    public class item
    {
        public string code { get; set; }
        public string description { get; set; }
        public List<chield> childs { get; set; }
    }
}
