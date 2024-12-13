using Colonos.Entidades;
using Colonos.Entidades.Drivin;
using Colonos.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Colonos.EndPoint.Controllers
{
    [RoutePrefix("api")]
    public class RutaController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //listar vehiculos
        [Route("vehiculos")]
        public IHttpActionResult ListVehiculos(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            ;
            var cnndrivin = setCnnDrivin();
            ManagerLogistica mng = new ManagerLogistica(logger, cnndrivin);
            var item = mng.ListVehiculos();

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

        private cnnDrivin setCnnDrivin()
        {
            var cnn = new cnnDrivin();
            cnn.baseurl = ConfigurationManager.AppSettings.Get("baseurldrivin");
            cnn.XAPIKey = ConfigurationManager.AppSettings.Get("XAPIKey");
            cnn.metodoscenarios = ConfigurationManager.AppSettings.Get("metodoscenarios");
            cnn.metodorutas = ConfigurationManager.AppSettings.Get("metodorutas");
            cnn.metodovehicles = ConfigurationManager.AppSettings.Get("metodovehicles");
            return cnn;
        }
    }
}
