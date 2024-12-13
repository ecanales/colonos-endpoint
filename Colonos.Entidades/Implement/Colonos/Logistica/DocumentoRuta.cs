using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class DocumentoRuta
    {
        public int DocEntry { get; set; }
        public string DocEstado { get; set; }
        public int DocTipo { get; set; }
        public string UsuarioCode { get; set; }
        public string Version { get; set; }
        public string UsuarioNombre { get; set; }
        public string EstadoOperativo { get; set; }
        public DateTime DocFecha { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Vehiculo { get; set; }
        public string scenario_token { get; set; }
        public string schema_name { get; set; }
        public string descripcion { get; set; }
        public List<Documento> clients { get; set; }
    }
}
