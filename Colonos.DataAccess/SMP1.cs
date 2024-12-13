//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Colonos.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class SMP1
    {
        public int DocEntry { get; set; }
        public int DocLinea { get; set; }
        public Nullable<int> DocTipo { get; set; }
        public string LineaEstado { get; set; }
        public string ProdCode { get; set; }
        public string ProdNombre { get; set; }
        public Nullable<decimal> PrecioFinal { get; set; }
        public Nullable<decimal> Descuento { get; set; }
        public Nullable<decimal> CantidadPendiente { get; set; }
        public Nullable<decimal> CantidadReal { get; set; }
        public Nullable<int> BaseEntry { get; set; }
        public Nullable<int> BaseLinea { get; set; }
        public Nullable<int> BaseTipo { get; set; }
        public string UsuarioCodeConfirma { get; set; }
        public Nullable<System.DateTime> FechaConfirma { get; set; }
        public Nullable<int> FamiliaCode { get; set; }
        public Nullable<int> AnimalCode { get; set; }
        public string TipoCode { get; set; }
        public string Medida { get; set; }
        public Nullable<int> FormatoVtaCode { get; set; }
        public Nullable<decimal> Margen { get; set; }
        public Nullable<decimal> Costo { get; set; }
        public Nullable<int> LineaItem { get; set; }
        public Nullable<decimal> Volumen { get; set; }
        public Nullable<decimal> CantidadSolicitada { get; set; }
        public Nullable<decimal> TotalSolicitado { get; set; }
        public Nullable<decimal> TotalReal { get; set; }
        public Nullable<decimal> Disponible { get; set; }
        public string BodegaCode { get; set; }
        public Nullable<int> RefrigeraCode { get; set; }
        public string RefrigeraNombre { get; set; }
        public Nullable<int> OrigenCode { get; set; }
        public string OrigenNombre { get; set; }
        public Nullable<int> MarcaCode { get; set; }
        public string MarcaNombre { get; set; }
        public string AnimalNombre { get; set; }
        public string FamiliaNombre { get; set; }
        public Nullable<decimal> SolicitadoAnterior { get; set; }
        public Nullable<decimal> CantidadEntregada { get; set; }
        public Nullable<decimal> Completado { get; set; }
    }
}