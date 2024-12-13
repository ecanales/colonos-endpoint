using Colonos.Manager;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Colonos.EndPoint.Controllers
{
    [RoutePrefix("api")]
    public class InformesController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] 
        [Route("informes/controlprecios")]
        public IHttpActionResult ConstrolPrecios(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var familiacode = query["familiacode"];


            if (familiacode == null || familiacode == "")
                familiacode = "-1";

            var mng = new ManagerInformes(logger);
            var item = mng.ControlPrecios(Convert.ToInt32(familiacode));
            if (!item.error)
            {
                return Ok(item);
            }
            else
            {
                logger.Error("mensaje: {0}. Data: {1}", item.msg, item.data);
                return ResponseMessage(Request.CreateResponse(item.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item))));

            }
        }
        [HttpGet]
        [Route("informes/seguimientooperacion")]
        public IHttpActionResult Get(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);

            var fechaini = query["fechaini"] ?? "";
            var fechafin = query["fechafin"] ?? "";
            var usuario = query["usuario"] ?? "";
            var cliente = query["cliente"] ?? "";

            //if (fechaini == null || fechaini == "") fechaini = "";
            //if (fechafin == null || fechafin == "") fechafin = "";
            //if (vendedor == null || vendedor == "") vendedor = "";
            //if (cliente == null || cliente == "") cliente = "";

            var mng = new ManagerInformes(logger);
            var item = mng.SeguimientoOperacion(usuario,fechaini, fechafin, cliente);
            if (!item.error)
            {
                return Ok(item);
            }
            else
            {
                logger.Error("mensaje: {0}. Data: {1}", item.msg, item.data);
                return ResponseMessage(Request.CreateResponse(item.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item))));

            }
        }

        //InfoPedidos
        [HttpGet]
        [Route("informes/resumenpedidosdeldia")]
        public IHttpActionResult ResumenPedidosDelDia(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var fecha = query["fecha"];


            if (fecha == null || fecha == "")
                fecha =String.Format("{0:yyyy-MM-dd}", DateTime.Now.Date);

            var mng = new ManagerInformes(logger);
            var item = mng.ResumenPedidosDelDia(Convert.ToDateTime(fecha));
            if (!item.error)
            {
                return Ok(item);
            }
            else
            {
                logger.Error("mensaje: {0}. Data: {1}", item.msg, item.data);
                return ResponseMessage(Request.CreateResponse(item.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item))));

            }
        }
    }
}
