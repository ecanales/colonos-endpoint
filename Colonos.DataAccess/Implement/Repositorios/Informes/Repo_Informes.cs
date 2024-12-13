using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_Informes
    {
        public string SeguimientoOperacion(string usuario, string fechaini, string fechafin, string cliente)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_SeguimientoOperacion(usuario,fechaini, fechafin,cliente) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string ControlPrecios(int familiacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_ControlPrecios(familiacode) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string PedidosDiarioVendedor(DateTime fecha)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_PedidosDiario_Vendedor(fecha) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
        public string PedidosVolumenesdeldia(DateTime fecha)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_PedidosDiario_TopClientes(fecha) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
        public string PedidosPorHoraArea(DateTime fecha)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_PedidosDiario_AreaPorHora(fecha) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
        public string PedidosPorHoraEjecutivo(DateTime fecha)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_PedidosDiario_VendedorPorHora(fecha) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
        public string PedidosEnCurso(DateTime fecha)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spInfo_PedidosDiario_Entregas(fecha) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
    }
}
