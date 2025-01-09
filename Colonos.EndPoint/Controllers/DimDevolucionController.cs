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
    public class DimDevolucionController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //busqueda
        [Route("devolucion/motivos")]
        public IHttpActionResult Search(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            
            ManagerDevoluciones mng = new ManagerDevoluciones(logger);

            var item = mng.ListMotivos();
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
