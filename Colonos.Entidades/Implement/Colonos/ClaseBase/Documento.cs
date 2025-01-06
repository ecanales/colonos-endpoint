using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Documento
    {
        public int DocEntry { get; set; }
        public string DocEstado { get; set; }
        public int? DocTipo { get; set; }
        public DateTime DocFecha { get; set; }
        public string SocioCode { get; set; }
        public string RazonSocial { get; set; }
        public int? ContactoCode { get; set; }
        public int? CondicionCode { get; set; }
        public int? DireccionCode { get; set; }
        public string VendedorCode { get; set; }
        public string VendedorNombre { get; set; }
        public string UsuarioCode { get; set; }
        public decimal? Neto { get; set; }
        public decimal? Iva { get; set; }
        public decimal? Total { get; set; }
        public string Observaciones { get; set; }
        public string EstadoOperativo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Version { get; set; }
        public string UsuarioNombre { get; set; }
        public string EstadoCliente { get; set; }
        public List<DocumentoLinea> Lineas { get; set; }
        public List<DocumentoLineaBandeja> Bandejas { get; set; }
        public List<Documento> DocumentoReferencia { get; set; }
        public DateTime FechaEntrega { get; set; }
        public bool? RetiraCliente { get; set; }
        public bool? AutorizacionEspecial { get; set; }
        public bool? AutorizadoEspecial { get; set; }
        public decimal? Costo { get; set; }
        public decimal? Margen { get; set; }
        public string BandejaCode { get; set; }
        public string MotivoRechazo { get; set; }
        public string CondicionNombre { get; set; }
        public int? BaseEntry { get; set; }
        public string BodegaCode { get; set; }
        public decimal? Completado { get; set; }
        public DateTime? FechaIngresoPrep { get; set; }
        public string Impreso { get; set; }
        public string UsuarioResponsable { get; set; }
        public string UsuarioCodeResponsable { get; set; }
        public string HorarioAtencion { get; set; }
        public decimal? CantidadPendiente { get; set; }
        public int? BaseTipo { get; set; }
        public int? BaseLinea { get; set; }
        public string clientFileDF { get; set; }
        public int? FolioDF { get; set; }
        public int? DireccionCodeFact { get; set; }
        public string scenario_token { get; set; }
        public string Vehiculo { get; set; }
        public int? TipoAjuste { get; set; }
        public string BodegaCodeOrigen { get; set; }
        public string BodegaCodeDestino { get; set; }
        public string CondicionDF { get; set; }
        public List<HistorialDoc> Historial { get; set; }
        public bool? Etiquetado { get; set; }
        public string Etiqueta { get; set; }
        public string DocEntryPK { get; set; }
        public string Instrucciones { get; set; }
        public string SubZona { get; set; }
        public string Zona { get; set; }
        public string Direccion { get; set; }
        public string DirObservaciones { get; set; }
        public string NumeroFormulario { get; set; }
        public string OCcliente { get; set; }
        public int? BaseRuta { get; set; }
        public int? BasePedido { get; set; }
        public string Descripcion { get; set; }
        public string RutaExitosa { get; set; }
        public string Custodio { get; set; }
        public int? BaseEntryCustodio { get; set; }
        public int? BaseTipoCustodio { get; set; }
        public string ObservacionesCierre { get; set; }
        public string TipoCustodio { get; set; }
        public int? BaseEntryLog { get; set; }
        public string TipoEntrega { get; set; }
        public string OtraEntrega { get; set; }

    }
}
