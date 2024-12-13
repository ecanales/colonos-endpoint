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
using System.Web.Http.Results;

namespace Colonos.EndPoint.Controllers
{
    [RoutePrefix("api")]
    public class ProductoController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //get_id
        [Route("productos/{prodcode}")]
        public IHttpActionResult Get(string prodcode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var item=mng.Get(prodcode);
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
        [Route("productos")]
        public IHttpActionResult List(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var activo = query["activo"] ?? "";

            if(activo==null)
            {
                activo = "";
            }

            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);

            
            var item = mng.List(activo, true);
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
        [Route("productos/desglose")]
        public IHttpActionResult ListDesglose(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var esdesglose = query["esdesglose"] ?? "";

            if (esdesglose == "")
                esdesglose = "S";

            var item = mng.List(esdesglose);
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
        [Route("productos/search")]
        public IHttpActionResult search(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var palabras= query["palabras"];
            var solorecetas = query["solorecetas"] ?? "" ;
            var maestro = query["maestro"] ?? "";
            var bodegacode = query["bodegacode"] ?? "";

            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            MensajeReturn item;

            if (maestro == "S")
            {
                item = mng.ListMaestro(palabras, solorecetas);
            }
            else
            {
                item = mng.List(palabras, solorecetas, bodegacode);
            }

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

        [HttpGet] //Consulta Stock
        [Route("productos/search/stock")]
        public IHttpActionResult searchStock(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var palabras = query["palabras"];
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            MensajeReturn item;

            item = mng.ListStock(palabras);

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
        [Route("productos/bodegas")]
        public IHttpActionResult bodegas(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var item = mng.ListBodegas();
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
        [Route("productos")]
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
                msg.data=jsonRequest;
                logger.Error("mensaje: {0}. Data: {1}", msg.msg, msg.data);
                return ResponseMessage(Request.CreateResponse(msg.statuscode, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(msg))));
            }
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
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
        [Route("productos")]
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
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
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

        [HttpGet] //listpropiedades
        [Route("productos/propiedades")]
        public IHttpActionResult propiedades(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
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

        [HttpGet] //listpropiedades
        [Route("productos/stock/all")]
        public IHttpActionResult UpdateStock(HttpRequestMessage request)
        {
            logger = NLog.LogManager.GetLogger("loggerfile2");
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var item = mng.UpdateStockDF();
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
        [Route("productos/costos/all")]
        public IHttpActionResult UpdateCostos(HttpRequestMessage request)
        {
            logger = NLog.LogManager.GetLogger("loggerfile3");
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var item = mng.UpdateCostoDF();
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

        [HttpPost] //actualziar stock en Colonos desde DF
        [Route("productos/stock/{bodegacodedf}")]
        public IHttpActionResult UpdateStockGroup(string bodegacodedf)
        {
            logger.Info("request {0}", Request.RequestUri);
            var jsonRequest =Request.Content.ReadAsStringAsync().Result;

            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);

            var list = JsonConvert.DeserializeObject<ConsultaStockDF>(jsonRequest);
            var item = mng.UpdateStockDF(list,bodegacodedf);
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

        [HttpGet] //actualziar stock en Colonos desde DF
        [Route("productos/stock")]
        public IHttpActionResult GetStockBodega(HttpRequestMessage request)
        {
            logger.Info("request {0}", request.RequestUri);
            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var prodcode = query["prodcode"];
            var bodegacode = query["bodegacode"];

            //
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var item = mng.GetStockBodega(prodcode, bodegacode);
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

        [HttpPost] //ajuste de inventario 
        [Route("productos/transaccion/traslado")]
        [Route("productos/transaccion/ajuste")]
        public IHttpActionResult AjusteInventario(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var jsonRequest = Request.Content.ReadAsStringAsync().Result;

            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var trx = JsonConvert.DeserializeObject<Transaccion>(jsonRequest);
            var item = mng.AjusteInventario(trx);
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


        [HttpGet] //list categorias
        [Route("productos/categorias")]
        public IHttpActionResult ListCategorias(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);
            var cnndf = setCnnDF();
            ManagerProductos mng = new ManagerProductos(logger, cnndf);
            var item = mng.ListCategorias();
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
