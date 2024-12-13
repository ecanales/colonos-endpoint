using Colonos.Entidades;
using Colonos.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class BandejasController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //list
        [Route("bandejas/{bandejacode}")]
        public IHttpActionResult List(string bandejacode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var mng = new ManagerBandejas(logger);

            //var param = Request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var estado = query["estado"];
            var visible = query["visible"];
            var sociocode = query["sociocode"];
            var top = query["top"]; 

            if (estado == null || estado == "")
                estado = "0";

            if (visible == null || visible == "")
                visible = "0";

            if (top == null || top == "")
                top = "0";

            MensajeReturn item;
            if (sociocode != null && sociocode.Length > 0)
            {
                if (top == "0")
                {
                    item = mng.List(bandejacode, sociocode);
                }
                else
                {
                    item = mng.List(bandejacode, sociocode, Convert.ToInt32(top));
                }
            }
            else
            {
                item = mng.List(bandejacode, Convert.ToBoolean(Convert.ToInt16(estado)), Convert.ToBoolean(Convert.ToInt16(visible)));
            }

            if (!item.error)
            {
                if(top!="0")
                {
                    //List<Bandeja> list=item.data;
                    //var top10 = list.OrderByDescending(o => o.FechaIngreso).Take(Convert.ToInt32(top));
                    //item.data = top10;
                    //return Ok(item);
                }
                return Ok(item);
            }
            else
            {
                logger.Error("mensaje: {0}. Data: {1}", item.msg, item.data);
                return ResponseMessage(Request.CreateResponse(item.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item))));

            }
        }


        [HttpGet] //get
        [Route("bandejas/{bandejacode}/{docentry}")]
        public IHttpActionResult Get(string bandejacode, int docentry)
        {
            logger.Info("request {0}", Request.RequestUri);
            var mng = new ManagerBandejas(logger);

            var item = mng.Get(bandejacode,docentry);

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

        [HttpPatch] //actualizar
        [Route("bandejas/{bandejacode}/{docentry}")]
        public IHttpActionResult Modify(string bandejacode, int docentry)
        {
            logger.Info("request {0}", Request.RequestUri);

            var jsonRequest = Request.Content.ReadAsStringAsync().Result;

            JObject json;

            try
            {
                json = JObject.Parse(jsonRequest);
            }
            catch
            {

                MensajeReturn msg = new MensajeReturn();
                msg.error = true;
                msg.statuscode = HttpStatusCode.BadRequest;
                msg.msg = "Json de entrada Incorrecto";
                msg.data = jsonRequest;
                logger.Error("mensaje: {0}. Data: {1}", msg.msg, msg.data);
                return ResponseMessage(Request.CreateResponse(msg.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(msg))));
            }

            var mng = new ManagerBandejas(logger);


            var item = mng.Modify(jsonRequest);

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
