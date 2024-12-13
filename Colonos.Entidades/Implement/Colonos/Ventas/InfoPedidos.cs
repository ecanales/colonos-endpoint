using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class InfoPedidos
    {
        public List<InfoPedidos_Diario_Vendedor> resumendia { get; set; }
        public List<InfoPedidos_TopClientes> velumenesdeldia { get; set; }
        public List<InfoPedidos_AreaPorHora> pedidosporhoraarea { get; set; }
        public List<InfoPedidos_VendedorPorHora> pedidosporhoraejecutivo { get; set; }
        public List<InfoPedidos_Entregas> pedidosencurso { get; set; }
    }
}
