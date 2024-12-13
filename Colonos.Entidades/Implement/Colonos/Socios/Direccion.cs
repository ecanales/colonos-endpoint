using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Direccion
    {
        public int DireccionCode { get; set; }
        public string DireccionTipo { get; set; }
        public string SocioCode { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public Nullable<int> ComunaCode { get; set; }
        public Nullable<int> CiudadCode { get; set; }
        public Nullable<int> RegionCode { get; set; }
        public string ComunaNombre { get; set; }
        public string CiudadNombre { get; set; }
        public string RegionNombre { get; set; }
        public string HorarioAtencion { get; set; }
        public string Observaciones { get; set; }
        public string EmailDriveIn { get; set; }
        public Nullable<bool> PorDefecto { get; set; }
        public string Ventana_Inicio { get; set; }
        public string Ventana_Termino { get; set; }
        public Nullable<decimal> Latitud { get; set; }
        public Nullable<decimal> Longitud { get; set; }
        public string SubZona { get; set; }
    }
}
