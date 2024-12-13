using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Producto
    {
        public string ProdCode { get; set; }
        public string ProdNombre { get; set; }
        public string Descripcion { get; set; }
        public string CategoriaCode { get; set; }
        public string TipoCode { get; set; }
        public int? FamiliaCode { get; set; }
        public  int? AnimalCode { get; set; }
        public int? FormatoCode { get; set; }
        public int? RefrigeraCode { get; set; }
        public string Activo { get; set; }
        public string SocioCode { get; set; }
        public int? MarcaCode { get; set; }
        public int? OrigenCode { get; set; }
        public string Medida { get; set; }
        public decimal? Costo { get; set; }
        public string EsDesglose { get; set; }
        public string TieneReceta { get; set; }
    }
}
