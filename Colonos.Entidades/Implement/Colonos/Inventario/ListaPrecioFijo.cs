using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class ListaPrecioFijo
    {
        
        public string ListaCode {get;set;}
        public string ListaNombre {get;set;}
        public DateTime Desde {get;set;}
        public DateTime Hasta {get;set;}
        public List<ListaPrecioFijoList> Lineas { get; set; }
    }
}
