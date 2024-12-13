using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Transaccion
    {
        
        public int DocEntry {get;set;}
        public DateTime DocFecha {get;set;}
        public int DocTipo {get;set;}
        public int TipoAjuste { get; set; }
        public string DocEstado {get;set;}
        public string BodegaCodeOrigen {get;set;}
        public string BodegaCodeDestino {get;set;}
        public DateTime FechaRegistro {get;set;}
        public int? FolioDF {get;set;}
        public string Observaciones { get; set; }
        public List<TransaccionLinea> Lineas { get;set;}
        
    }
}
