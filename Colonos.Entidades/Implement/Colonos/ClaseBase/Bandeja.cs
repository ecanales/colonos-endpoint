using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class Bandeja
    {
        public int DocEntry { get; set; }
        public string BandejaCode { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string UsuarioCodeIngreso { get; set; }
        public string SocioCode { get; set; }
        public string RazonSocial { get; set; }
        public string VendedorCode { get; set; }
        public DateTime? FechaAproRech { get; set; }
        public string MotivoRech { get; set; }
        public string UsuarioCodeAproRech { get; set; }
        public List<ItemBandeja> Items { get; set; }
        public bool? Visible { get; set; }
        public bool? Autorizado { get; set; }
    }
}
