using Colonos.Entidades;
using Colonos.Entidades.Defontana;
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
    public class DimProductosController : ApiController
    {
        Logger logger = LogManager.GetLogger("loggerfile");

        #region ITM4 FAMILIA ************

        [HttpPost]
        [Route("productos/dim/familia")]
        public IHttpActionResult AddFamilia(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();

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

            ManagerFamilia mng = new ManagerFamilia(logger);


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

        [HttpPatch]
        [Route("productos/dim/familia")]
        public IHttpActionResult ModifyFamilia(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();

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

            ManagerFamilia mng = new ManagerFamilia(logger);


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

        [HttpGet] //list
        [Route("productos/dim/familia/{familiacode}")]
        public IHttpActionResult GetFamilia(int familiacode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerFamilia mng = new ManagerFamilia(logger);


            var item = mng.Get(familiacode);
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

        [HttpGet] //list
        [Route("productos/dim/familia")]
        public IHttpActionResult ListFamilia(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var palabras = query["palabras"];

            if (palabras == null)
                palabras = "";
            var cnndf = setCnnDF();
            ManagerFamilia mng = new ManagerFamilia(logger);


            MensajeReturn item;
            if(palabras=="")
                item = mng.List();
            else
                item = mng.List(palabras);

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



        #endregion iITM4 FAMILIA ***************************


        #region ITM5 ANIMAL ************

        [HttpPost] 
        [Route("productos/dim/animal")]
        public IHttpActionResult AddAnimal(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();

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

            ManagerAnimal mng = new ManagerAnimal(logger);


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

        [HttpPatch]
        [Route("productos/dim/animal")]
        public IHttpActionResult ModifyAnimal(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();

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

            ManagerAnimal mng = new ManagerAnimal(logger);


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

        [HttpGet] //list
        [Route("productos/dim/animal/{animalcode}")]
        public IHttpActionResult GetAnimal(int animalcode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerAnimal mng = new ManagerAnimal(logger);


            var item = mng.Get(animalcode);
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

        [HttpGet] //list
        [Route("productos/dim/animal")]
        public IHttpActionResult ListAnimal(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerAnimal mng = new ManagerAnimal(logger);


            var item = mng.List();
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

        #endregion iITM5 ANIMAL ***************************

        private cnnDF setCnnDF()
        {
            var cnndf = new cnnDF();
            cnndf.baseurl = ConfigurationManager.AppSettings.Get("baseurl");
            cnndf.cliente = ConfigurationManager.AppSettings.Get("cliente");
            cnndf.company = ConfigurationManager.AppSettings.Get("company");
            cnndf.user = ConfigurationManager.AppSettings.Get("user");
            cnndf.pass = ConfigurationManager.AppSettings.Get("pass");
            cnndf.docfactura = ConfigurationManager.AppSettings.Get("docfactura");
            cnndf.docingreso = ConfigurationManager.AppSettings.Get("docingreso");
            cnndf.docegreso = ConfigurationManager.AppSettings.Get("docegreso");
            cnndf.doctraslado = ConfigurationManager.AppSettings.Get("doctraslado");
            cnndf.metodoajustes = ConfigurationManager.AppSettings.Get("metodoajustes");
            cnndf.metodotraslados = ConfigurationManager.AppSettings.Get("metodotraslados");
            cnndf.metodofacturas = ConfigurationManager.AppSettings.Get("metodofacturas");
            cnndf.metodostockgroup = ConfigurationManager.AppSettings.Get("metodostockgroup");
            cnndf.metodostockbodega = ConfigurationManager.AppSettings.Get("metodostockbodega");
            cnndf.metodocosto = ConfigurationManager.AppSettings.Get("metodocosto");
            cnndf.accclientesnacionales = ConfigurationManager.AppSettings.Get("accclientesnacionales");
            cnndf.acciva = ConfigurationManager.AppSettings.Get("acciva");
            cnndf.valoriva = Convert.ToDecimal(ConfigurationManager.AppSettings.Get("valoriva"));
            cnndf.metodoauth = ConfigurationManager.AppSettings.Get("metodoauth");
            cnndf.accventaingresos = ConfigurationManager.AppSettings.Get("accventaingresos");

            return cnndf;
        }
    }
}
