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
    public class PrecioFijoController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //busqueda
        [Route("preciofijo/list")]
        public IHttpActionResult ListarPreciosFijos(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);

            
            ManagerPreciosFijos mng = new ManagerPreciosFijos(logger);
            MensajeReturn item;

            item = mng.List();


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

        [HttpPost] //nuevo
        [Route("preciofijo")]
        public IHttpActionResult Add(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var jsonRequest = request.Content.ReadAsStringAsync().Result;

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

            ManagerPreciosFijos mng = new ManagerPreciosFijos(logger);
            var item = mng.Add(jsonRequest);
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
        [Route("preciofijo")]
        public IHttpActionResult Modify(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var jsonRequest = request.Content.ReadAsStringAsync().Result;

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

            ManagerPreciosFijos mng = new ManagerPreciosFijos(logger);
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

        [HttpDelete] //delete
        [Route("preciofijo/{listacode}")]
        public IHttpActionResult Delete(string listacode)
        {
            logger.Info("request {0}", Request.RequestUri);

            var jsonRequest = Request.Content.ReadAsStringAsync().Result;

            JObject json;

            //try
            //{
            //    json = JObject.Parse(jsonRequest);
            //}
            //catch
            //{

            //    MensajeReturn msg = new MensajeReturn();
            //    msg.error = true;
            //    msg.statuscode = HttpStatusCode.BadRequest;
            //    msg.msg = "Json de entrada Incorrecto";
            //    msg.data = jsonRequest;
            //    logger.Error("mensaje: {0}. Data: {1}", msg.msg, msg.data);
            //    return ResponseMessage(Request.CreateResponse(msg.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(msg))));
            //}

            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var prodcodigo = query["prodcodigo"];
            if (prodcodigo == null)
                prodcodigo = "";

            ManagerPreciosFijos mng = new ManagerPreciosFijos(logger);
            var item = mng.Delete(listacode, prodcodigo);
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

        [HttpGet] //busqueda
        [Route("preciofijo/search")]
        public IHttpActionResult search(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var palabras = query["palabras"];

            ManagerPreciosFijos mng = new ManagerPreciosFijos(logger);
            var item = mng.List(palabras, "");
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

        [HttpGet] //busqueda
        [Route("preciofijo/{listacode}")]
        public IHttpActionResult search(string listacode)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = Request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var palabras = query["palabras"];

            ManagerPreciosFijos mng = new ManagerPreciosFijos(logger);
            var item = mng.Get(listacode);
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
