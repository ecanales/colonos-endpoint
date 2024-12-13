using Colonos.Entidades;
using Colonos.Entidades.Defontana;
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
    public class DocumentoController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        

        [HttpGet] //busqueda
        [Route("documentos/search")]
        public IHttpActionResult Search(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var palabras = query["palabras"];
            var code = query["code"];
            var doctipo = query["doctipo"];
            var usuario = query["usuario"] ?? "";
            if (code == null)
                code = "";
            if (doctipo == null || doctipo=="")
                doctipo = "0";
            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerDocumentos mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            var item = mng.Search(palabras, code,Convert.ToInt32(doctipo), usuario);
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
        [Route("documentos")]
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
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
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
        [Route("documentos")]
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

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
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

        [HttpPatch] //actualizar
        [Route("documentos/sets")]
        public IHttpActionResult ModifySets(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var jsonRequest = request.Content.ReadAsStringAsync().Result;

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

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            var item = mng.ModifySets(jsonRequest);
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
        [Route("documentos/etiqueta")]
        public IHttpActionResult ModifyEtiqueta(HttpRequestMessage request)
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
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            var item = mng.ModifyEtiqueta(jsonRequest);
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
        [Route("documentos/ordendecompra")]
        public IHttpActionResult ModifyOrdendeCompra(HttpRequestMessage request)
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
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            var item = mng.ModifyOrdendeCompra(jsonRequest);
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

        [HttpPatch] //actualizar estado item
        [Route("documentos/estadoitem")]
        public IHttpActionResult ModifyEstadoItem(HttpRequestMessage request)
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

            logger.Info("ModifyEstadoItem. json entrada: {0}", jsonRequest);

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            var item = mng.ModifyEstadoItem(jsonRequest);
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

        [HttpGet] //get_id
        [Route("documentos/{docentry}")]
        public IHttpActionResult Get(string docentry)
        {
            logger.Info("request {0}", Request.RequestUri);

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf,cnndrivin);
            if (docentry == null || docentry == "")
                docentry = "0";

            var param = Request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var bodegacode = query["bodegacode"];
            var doctipo = query["doctipo"];

            if (doctipo == null || doctipo == "")
                doctipo = "0";

            var item = mng.Get(Convert.ToInt32(docentry), Convert.ToInt32(doctipo));
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



        [HttpGet] //get_id
        [Route("documentos/base/{baseentry}")]
        public IHttpActionResult GetBase(string baseentry)
        {
            logger.Info("request {0}", Request.RequestUri);

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger,cnndf, cnndrivin);
            if (baseentry == null || baseentry == "")
                baseentry = "0";

            var param = Request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var bodegacode = query["bodegacode"];
            var doctipo = query["doctipo"];

            if (doctipo == null || doctipo == "")
                doctipo = "0";

            var item = mng.GetBase(Convert.ToInt32(baseentry), Convert.ToInt32(doctipo));
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

        [HttpGet] //get_id
        [Route("documentos/base/linea/{baseentry}")]
        public IHttpActionResult GetBaseLinea(string baseentry)
        {
            logger.Info("request {0}", Request.RequestUri);

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            if (baseentry == null || baseentry == "")
                baseentry = "0";

            var param = Request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var bodegacode = query["bodegacode"];
            var doctipo = query["doctipo"];
            var baselinea = query["baselinea"];

            if (doctipo == null || doctipo == "")
                doctipo = "0";

            if (baselinea == null || baselinea == "")
                baselinea = "0";

            var item = mng.GetBaseLinea(Convert.ToInt32(baseentry), Convert.ToInt32(doctipo), Convert.ToInt32(baselinea));
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

        [HttpGet] //get_propiedades
        [Route("documentos/propiedades")]
        public IHttpActionResult Propiedades(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            var mng = new ManagerDocumentos(logger,cnndf, cnndrivin);

            var param = Request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var doctipo = query["doctipo"];

            if (doctipo == null || doctipo == "")
                doctipo = "0";

            var item = mng.Propiedades(Convert.ToInt32(doctipo));
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
        [Route("documentos/list")]
        public IHttpActionResult List(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var bodegacode = query["bodegacode"] ?? "";
            var doctipo = query["doctipo"] ?? "0";
            var estado = query["estado"] ?? "";
            var estadooperativo = query["estadooperativo"] ?? "";
            var vendedorcode = query["vendedorcode"] ?? "";
            var pendiente = query["pendiente"] ?? "-1";
            var desde = query["desde"] ?? "";
            var hasta = query["hasta"] ?? "";
            var usuario = query["usuario"] ?? "";

            if (doctipo == null || doctipo == "")
                doctipo = "0";

            if (pendiente == null)
                pendiente = "-1";

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerDocumentos mng = new ManagerDocumentos(logger,cnndf, cnndrivin);
            var item = mng.List(Convert.ToInt32(doctipo), bodegacode,estadooperativo,vendedorcode,estado, Convert.ToInt32(pendiente), desde, hasta, usuario);
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
        [Route("documentos/listpicking")]
        public IHttpActionResult ListPiking(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var bodegacode = query["bodegacode"];
            var estado = query["estado"];

            if (estado == null || estado == "")
                estado = "A";

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerDocumentos mng = new ManagerDocumentos(logger,cnndf, cnndrivin);
            var item = mng.ListPicking(bodegacode, estado );
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

        [HttpGet] //picking
        [Route("documentos/picking/produccion")]
        public IHttpActionResult PickingProduccion(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var docentry = query["docentry"];


            if (docentry == null || docentry == "")
                docentry = "0";

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerPicking mng = new ManagerPicking(logger, cnndf);
            var item = mng.Picking(Convert.ToInt32(docentry),"produccion");
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

        [HttpGet] //picking
        [Route("documentos/picking/toledo")]
        public IHttpActionResult PickingToledo(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var docentry = query["docentry"];


            if (docentry == null || docentry == "")
                docentry = "0";

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerPicking mng = new ManagerPicking(logger, cnndf);
            var item = mng.Picking(Convert.ToInt32(docentry),"toledo");
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

        

        [HttpGet] //listar pedidos en preparacion bodega Toledo o Producción
        [Route("documentos/preparacion/list")]
        public IHttpActionResult ListBodegabodega(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var bodegacode = query["bodegacode"];
            var doctipo = query["doctipo"];

            if (doctipo == null || doctipo == "")
                doctipo = "0";

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerDocumentos mng = new ManagerDocumentos(logger,cnndf, cnndrivin);
            var item = mng.List(Convert.ToInt32(doctipo), bodegacode);
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

        [HttpGet] //historial documento
        [Route("documentos/historial")]
        public IHttpActionResult Historial(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var docentry = query["docentry"];
            var doctipo = query["doctipo"];

            if (doctipo == null || doctipo == "")
                doctipo = "0";
            if (docentry == null || docentry == "")
                docentry = "0";

            var cnndf = setCnnDF();
            var cnndrivin = setCnnDrivin();
            ManagerDocumentos mng = new ManagerDocumentos(logger, cnndf, cnndrivin);
            var item = mng.Historial(Convert.ToInt32(docentry), Convert.ToInt32(doctipo));
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

            return cnndf;
        }

        private cnnDrivin setCnnDrivin()
        {
            var cnn = new cnnDrivin();
            cnn.baseurl = ConfigurationManager.AppSettings.Get("baseurldrivin");
            cnn.XAPIKey = ConfigurationManager.AppSettings.Get("XAPIKey");
            cnn.metodoscenarios = ConfigurationManager.AppSettings.Get("metodoscenarios");
            cnn.metodorutas = ConfigurationManager.AppSettings.Get("metodorutas");
            cnn.metodovehicles = ConfigurationManager.AppSettings.Get("metodovehicles");
            cnn.schema_name = ConfigurationManager.AppSettings.Get("schema_name");

            return cnn;
        }
    }
}
