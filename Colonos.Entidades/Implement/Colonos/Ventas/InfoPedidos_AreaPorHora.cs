﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class InfoPedidos_AreaPorHora
    {
        public string IdGrupo { get; set; }
        public Nullable<int> Pedidos { get; set; }
        public Nullable<decimal> PedidosporHora { get; set; }
        public Nullable<int> HorasTurno { get; set; }
    }
}
