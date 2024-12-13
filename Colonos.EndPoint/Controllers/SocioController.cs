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
    public class SocioController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");
        
        [HttpGet] //get_id
        [Route("socios/{sociocode}")]
        public IHttpActionResult Get(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);
            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.Get(sociocode);
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
        [Route("socios/list")]
        public IHttpActionResult List(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            ManagerSocios mng = new ManagerSocios(logger);
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

        [HttpGet] //busqueda
        [Route("socios/search")]
        public IHttpActionResult search(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var palabras = query["palabras"];
            var tipo = query["tipo"];
            var usuario = query["usuario"];
            if (tipo == null)
                tipo = "";
            if (usuario == null)
                usuario = "";
            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.List(palabras, tipo, usuario);
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
        [Route("socios")]
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

            ManagerSocios mng = new ManagerSocios(logger);
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


        [HttpPost] //nuevo
        [Route("socios")]
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

            ManagerSocios mng = new ManagerSocios(logger);
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

        [HttpPost] //nuevo
        [Route("socios/addDF")]
        public IHttpActionResult AddDF(HttpRequestMessage request)
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

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.AddDF(jsonRequest);
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

        [HttpDelete] //get_id
        [Route("socios/{sociocode}")]
        public IHttpActionResult Delete(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);
            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.Delete(sociocode);
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

        [HttpGet] //listpropiedades
        [Route("socios/propiedades")]
        public IHttpActionResult propiedades(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.Propiedades();
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

        [HttpGet] //list Top 10 Familai Cliente
        [Route("socios/{sociocode}/TopFamilia")]
        public IHttpActionResult TopFamiliaCliente(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.TopFamiliaCliente(sociocode);
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

        [HttpGet] //list Top 10 ventas Cliente
        [Route("socios/{sociocode}/TopVentas")]
        public IHttpActionResult TopVentasCliente(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.TopVentasCliente(sociocode);
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

        [HttpGet] //list Top 10 Familai Cliente
        [Route("socios/{rubro}/TopRubro")]
        public IHttpActionResult TopFamiliaRubro(string rubro)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.TopFamiliaRubro(rubro);
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

        [HttpGet] //list Top 10 Precios Familia Cliente
        [Route("socios/{sociocode}/{familiacode}/TopUltimosPrecios")]
        public IHttpActionResult TopUltimosPrecios(string sociocode, string familiacode)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.TopUltimosPrecios(sociocode, familiacode);
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

        [HttpGet] //list cuenta corriente Cliente
        [Route("socios/{sociocode}/cuentacorriente")]
        public IHttpActionResult CuentaCorriente(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.CuentaCorriente(sociocode);
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

        [HttpGet] //obtener cliente desde DF
        [Route("socios/cuenta/{sociocode}")]
        public IHttpActionResult GetClienteDefontana(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.CuentaCorriente(sociocode);
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


        [HttpGet] //obtener ventas cliente desde DF
        [Route("socios/ventas12m/{sociocode}/")]
        public IHttpActionResult Ventas12Meses(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);

            ManagerSocios mng = new ManagerSocios(logger);
            var item = mng.Ventas12Meses(sociocode);
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

        [HttpGet] //obtener cliente desde DF
        [Route("socios/defontana/{sociocode}")]
        public IHttpActionResult GetClienteDF(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();

            ManagerSocios mng = new ManagerSocios(logger,cnndf);
            var item = mng.GetClienteDefontana(sociocode);
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
            cnndf.accclientesnacionales = ConfigurationManager.AppSettings.Get("accclientesnacionales");
            cnndf.acciva = ConfigurationManager.AppSettings.Get("acciva");
            cnndf.valoriva = Convert.ToDecimal(ConfigurationManager.AppSettings.Get("valoriva"));
            cnndf.metodoauth = ConfigurationManager.AppSettings.Get("metodoauth");
            cnndf.accventaingresos = ConfigurationManager.AppSettings.Get("accventaingresos");
            cnndf.metodoclientes= ConfigurationManager.AppSettings.Get("metodoclientes");

            return cnndf;
        }
    }
}
