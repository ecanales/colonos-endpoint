using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class ProductosResult
    {

        public string ProdCode {get;set;}
        public string ProdNombre {get;set;}
        public string Tipo {get;set;}
        public string MedidaNombre {get;set;}
        public string FamiliaNombre {get;set;}
        public string AnimalNombre {get;set;}
        public string FormatoNombre { get;set;}
        public string RefrigeraNombre {get;set;}
        public string FrmtoVentaNombre { get;set;}
        public string MarcaNombre { get; set; }
        public string OrigenNombre { get; set; }
        public decimal Costo {get;set;}
        public string BodegaCode {get;set;}
        public decimal Stock {get;set;}
        public decimal Asignado {get;set;}
        public decimal Disponible {get;set;}
        public decimal Factorprecio {get;set;}
        public decimal DescVolumen {get;set;}
        public decimal Volumen {get;set;}
        public decimal Margen {get;set;}
        public decimal PrecioUnitario {get;set;}
        public decimal PrecioVolumen {get;set;}
        public int? RefrigeraCode { get; set; }
        public int? AnimalCode { get; set; }
        public int? FamiliaCode { get; set; }
        public int? FormatoCode { get; set; }
        public int? FormatoVtaCode { get; set; }
        public int? MarcaCode { get; set; }
        public int? OrigenCode { get; set; }
        public string Activo { get; set; }
        public string EsDesglose { get; set; }
        public decimal? PrecioFijo { get; set; }
        public string TieneReceta { get; set; }
        public decimal Almafrigo { get; set; }
        public decimal Produccion { get; set; }
        public decimal Toledo { get; set; }
        public decimal StockTotal { get; set; }
        public decimal DisponibleReceta { get; set; }
        public string Icestar { get; set; }
        public decimal StockToledo { get; set; }
        public decimal AsignadoToledo { get; set; }

    }
}
