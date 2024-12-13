using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Picking_Lineas
    {
        
        public int DocEntry {get;set;}
        public int BaseTipo {get;set;}
        public string TipoCode {get;set;}
        public string ProdCode {get;set;}
        public string ProdNombre {get;set;}
        public string MarcaNombre {get;set;}
        public string RefrigeraNombre {get;set;}
        public decimal CantidadSolicitada {get;set;}
        public decimal Stock {get;set;}
        public string ProdCodeRe {get;set;}
        public string ProdNombreRe {get;set;}
        public string TipoCodeRe {get;set;}
        public string MarcaNombreRe {get;set;}
        public string RefrigeraNombreRe {get;set;}
        public decimal StockRe {get;set;}
        public string BodegaCode {get;set;}
        public decimal Costo { get; set; }
        public decimal CostoRe { get; set; }
        public int DocLinea { get; set; }
        public decimal PrecioFinal { get; set; }
        public decimal Disponible { get; set; }
        public decimal CantidadPendiente { get; set; }
    }
}
