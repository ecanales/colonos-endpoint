using Colonos.DataAccess;
using Colonos.Entidades;
using Colonos.Entidades.Defontana;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Manager
{
    public class ManagerInformes
    {
        Logger logger;
        public ManagerInformes(Logger _logger)
        {
            logger = _logger;
        }
        public MensajeReturn SeguimientoOperacion(string usuario, string fechaini, string fechafin, string cliente)
        {
            var repo = new Repo_Informes();
            var json = repo.SeguimientoOperacion(usuario,fechaini, fechafin, cliente); 
            var list = JsonConvert.DeserializeObject<List<spInfo_SeguimientoOperacion_Result>>(json); 

            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Seguimiento Operacion";
            msg.data = list;
            return msg;
        }

        public MensajeReturn ControlPrecios(int familiacode)
        {
            var repo = new Repo_Informes();
            var json = repo.ControlPrecios(familiacode);
            var list = JsonConvert.DeserializeObject<List<spInfo_ControlPrecios_Result>>(json);

            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Control Precios";
            msg.data = list;
            return msg;
        }

        public MensajeReturn ResumenPedidosDelDia(DateTime fecha)
        {
            
            
            var repo = new Repo_Informes();
            var json = repo.PedidosDiarioVendedor(fecha);
            var resumen = JsonConvert.DeserializeObject<List<InfoPedidos_Diario_Vendedor>>(json);
            json = repo.PedidosVolumenesdeldia(fecha);
            var topclientes = JsonConvert.DeserializeObject<List<InfoPedidos_TopClientes>>(json);
            json = repo.PedidosPorHoraArea(fecha);
            var pedidoporhoraarea = JsonConvert.DeserializeObject<List<InfoPedidos_AreaPorHora>>(json);
            json = repo.PedidosPorHoraEjecutivo(fecha);
            var pedidoporhoraejecutivo= JsonConvert.DeserializeObject<List<InfoPedidos_VendedorPorHora>>(json);
            json = repo.PedidosEnCurso(fecha);
            var pedidoencurso= JsonConvert.DeserializeObject<List<InfoPedidos_Entregas>>(json);

            var list = new InfoPedidos
            {
                resumendia = resumen,
                velumenesdeldia = topclientes,
                pedidosporhoraarea = pedidoporhoraarea,
                pedidosporhoraejecutivo = pedidoporhoraejecutivo,
                pedidosencurso = pedidoencurso
            };

            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.resumendia.Count();
            msg.msg = "Info Pedidos Diario";
            msg.data = list;
            return msg;
        }
    }
}
