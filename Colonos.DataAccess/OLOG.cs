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
    
    public partial class OLOG
    {
        public int DocEntry { get; set; }
        public string DocEstado { get; set; }
        public Nullable<int> DocTipo { get; set; }
        public Nullable<System.DateTime> DocFecha { get; set; }
        public string SocioCode { get; set; }
        public Nullable<int> ContactoCode { get; set; }
        public Nullable<int> CondicionCode { get; set; }
        public Nullable<int> DireccionCode { get; set; }
        public string VendedorCode { get; set; }
        public string UsuarioCode { get; set; }
        public Nullable<decimal> Neto { get; set; }
        public Nullable<decimal> Iva { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Observaciones { get; set; }
        public string EstadoOperativo { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
        public string Version { get; set; }
        public string UsuarioNombre { get; set; }
        public string EstadoCliente { get; set; }
        public string RazonSocial { get; set; }
        public Nullable<System.DateTime> FechaEntrega { get; set; }
        public Nullable<bool> RetiraCliente { get; set; }
        public Nullable<bool> AutorizacionEspecial { get; set; }
        public Nullable<decimal> Costo { get; set; }
        public Nullable<decimal> Margen { get; set; }
        public string BandejaCode { get; set; }
        public string MotivoRechazo { get; set; }
        public Nullable<bool> AutorizadoEspecial { get; set; }
        public string CondicionNombre { get; set; }
        public Nullable<System.DateTime> FechaIngresoPrep { get; set; }
        public Nullable<decimal> Completado { get; set; }
        public Nullable<int> BaseEntry { get; set; }
        public Nullable<int> BaseTipo { get; set; }
        public string Vehiculo { get; set; }
        public string scenario_token { get; set; }
        public Nullable<int> BaseRuta { get; set; }
        public string clientFileDF { get; set; }
        public Nullable<int> FolioDF { get; set; }
        public Nullable<int> DireccionCodeFact { get; set; }
        public string CondicionDF { get; set; }
        public Nullable<int> BasePedido { get; set; }
        public string RutaExitosa { get; set; }
        public string Custodio { get; set; }
        public Nullable<int> BaseEntryCustodio { get; set; }
        public Nullable<int> BaseTipoCustodio { get; set; }
        public string ObservacionesCierre { get; set; }
        public string TipoCustodio { get; set; }
        public string TipoEntrega { get; set; }
        public string OtraEntrega { get; set; }
    }
}
