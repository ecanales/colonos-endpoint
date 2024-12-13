using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Picking
    {
        
        public int DocEntry {get;set;}
        public string DocumentoOrigen {get;set;}
        public int? NumeroDocumento {get;set;}
        public int? DocEntrySol {get;set;}
        public int? DocEntryOv {get;set;}
        public string DocEstado {get;set;}
        public string EstadoActual {get;set;}
        public DateTime DocFecha {get;set;}
        public string RazonSocial {get;set;}
        public string VendedorCode {get;set;}
        public DateTime FechaRegistro {get;set;}
        public string UsuarioResponsable {get;set;}
        public string RetiraCliente {get;set;}
        public int BaseTipo {get;set;}
        public List<Picking_Lineas> Lineas { get;set;}
        public string DocOrigen { get;set;}
        public string Instrucciones { get; set; }
        public string FechaEntrega { get; set; }
        public string Horario { get; set; }

        public string Calle { get; set; }
        public string ComunaNombre { get; set; }
        public string Observaciones { get; set; }
        public string SubZona { get; set; }
        public string CondicionDF { get; set; }
    }
}
