using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class DocumentoLinea
    {
        public int DocEntry { get; set; }
        public int DocLinea { get; set; }
        public int LineaItem { get; set; }
        public int? DocTipo { get; set; }
        public string LineaEstado { get; set; }
        public string ProdCode { get; set; }
        public string ProdNombre { get; set; }
        public decimal? CantidadSolicitada { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? PrecioVolumen { get; set; }
        public decimal? FactorPrecio { get; set; }
        public decimal? MargenRegla { get; set; }
        public decimal? Disponible { get; set; }
        public string BodegaCode { get; set; }
        public decimal? PrecioFinal { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? CantidadPendiente { get; set; }
        public decimal? CantidadReal { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLinea { get; set; }
        public int? FamiliaCode { get; set; }
        public int? AnimalCode { get; set; }
        public string TipoCode { get; set; }
        public string Medida { get; set; }
        public decimal? Margen { get; set; }
        public decimal? Costo { get; set; }
        public int FormatoVtaCode { get; set; }
        public string UsuarioCodeConfirma { get; set; }
        public DateTime? FechaConfirma { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? TotalSolicitado { get; set; }
        public decimal? TotalReal { get; set; }
        public int? RefrigeraCode { get; set; }
        public int? MarcaCode { get; set; }
        public int? OrigenCode { get; set; }
        public string RefrigeraNombre { get; set; }
        public string MarcaNombre { get; set; }
        public string OrigenNombre { get; set; }
        public string FrmtoVentaNombre { get; set; }
        public string AnimalNombre { get; set; }
        public string FamiliaNombre { get; set; }
        public decimal? SolicitadoAnterior { get; set; }
        public decimal? CantidadEntregada { get; set; }
        public decimal? Completado { get; set; }
        public string TieneReceta { get; set; }
        public string ProdTipo { get; set; }
        public int? Cajas { get; set; }
        public decimal? KilosPorCaja { get; set; }
        public decimal? Diferencia { get; set; }
        public decimal? Tolerancia { get; set; }
        public int? BaseTipo { get; set; }
        public string ProdCodeRefGlosa { get; set; }
        public decimal? StockActual { get; set; }
        public List<RecetaProductoList> Receta { get; set; }
        public decimal? StockReceta { get; set; }
        public decimal? StockRecetaMP { get; set; }
        public decimal? CantidadRendida { get; set; }
        public string ProdCodeRef { get; set; }
        public string ProdNombreRef { get; set; }
        public string EnProduccion { get; set; }
    }
}
