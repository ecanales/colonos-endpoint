using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class DocumentosResult
    {
        public int? BaseEntry { get; set; }
        public int? BaseTipo { get; set; }
        public int DocEntry {get;set;}
        public int DocTipo { get; set; }
        public string SocioCode {get;set;}
        public string RazonSocial {get;set;}
        public DateTime DocFecha {get;set;}
        public string DocEstado {get;set;}
        public decimal? Neto {get;set;}
        public decimal? Total { get; set; }
        public decimal? TotalFacturable { get; set; }
        public decimal? TotalKilos { get; set; }
        public decimal? TotalKilosPendientes { get; set; }
        public string EstadoOperativo {get;set;}
        public string VendedorCode {get;set;}
        public string VendedorNombre { get; set; }
        public string Observaciones { get; set; }
        public string RetiraCliente { get; set; }
        public decimal? Completado { get; set; }
        public decimal? CompletadoTol { get; set; }
        public string BodegaCode { get; set; }
        public DateTime? FechaIngresoPrep { get; set; }
        public string Impreso { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string UsuarioResponsable { get; set; }
        public string HorarioAtencion { get; set; }
        public decimal? CantidadPendiente { get; set; }
        public int? Pedido { get; set; }
        public int? FolioDF { get; set; }
        public string Direccion { get; set; }
        public string ComunaNombre { get; set; }
        public string CiudadNombre { get; set; }
        public string Zona { get; set; }
        public string SubZona { get; set; }
        public string Ventana_Inicio { get; set; }
        public string Ventana_Termino { get; set; }
        public string Ventana { get; set; }
        public decimal? Longitud { get; set; }
        public decimal? Latitud { get; set; }
        public string tokenRuta { get; set; }
        public decimal? TotalKilosReales { get; set; }
        public decimal? TotalKilosFacturados { get; set; }
        public int? TipoAjuste { get; set; }
        public string BodegaCodeOrigen { get; set; }
        public string BodegaCodeDestino { get; set; }
        public string CondicionDF { get; set; }
        public string DocOrigen { get; set; }
        public int? EntryOrigen { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public bool? Etiquetado { get; set; }
        public string Etiqueta { get; set; }
        public string DocEntryPK { get; set; }
        public string Instrucciones { get; set; }
        public string UsuarioNombre { get; set; }
        public string NumeroFormulario { get; set; }
        public decimal TotalKilosA { get; set; }
        public decimal TotalKilosB { get; set; }
        public string TieneB { get; set; }
        public string UltimoPK { get; set; }
        public decimal TotalKilosPorMarcar { get; set; }
        public decimal TotalKilosSolicitados { get; set; }
        public decimal TotalKilosPorFacturar { get; set; }
        public decimal TotalPendiente { get; set; }
        public int? BaseRuta { get; set; }
        public string Vehiculo { get; set; }
        public string scenario_token { get; set; }
        public string Descripcion { get; set; }
        public string TipoCustodio { get; set; }
        public int? BaseEntryLog { get; set; }
        public int? BaseEntryCustodio { get; set; }
        public int? BaseTipoCustodio { get; set; }
        public string ObservacionesCierre { get; set; }
        public string TipoEntrega { get; set; }
        public string OtraEntrega { get; set; }
        public string TipoProblema { get; set; }
        public string Devolucion { get; set; }
        public string Motivo { get; set; }
    }
}
