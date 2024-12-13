using Colonos.DataAccess;
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
    public class UsuarioController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //list
        [Route("usuarios/list")]
        public IHttpActionResult List(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var idgrupo = query["idgrupo"] ?? "";
            var idgrupo2 = query["idgrupo2"] ?? "";

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            var item = mng.ListarUsuario(idgrupo, idgrupo2);
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
        [Route("system/users/all")]
        public IHttpActionResult GetUsuarios(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var param = request.RequestUri.Query.Split('=');
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var idgrupo = query["idgrupo"] ?? "";

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            var item = mng.ListarUsuario(idgrupo,"");
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

        [HttpPost]
        [Route("system/users/add")]
        public IHttpActionResult AddUsuario(HttpRequestMessage request)
        {

            var jsonRequest = request.Content.ReadAsStringAsync().Result;

            OUSR us = JsonConvert.DeserializeObject<OUSR>(jsonRequest);

            var re = Request;
            var headers = re.Headers;
            //var IdUsuario = re.RequestUri.Segments[4];

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            string msg = mng.AddUsuario(us);
            User usuario = JsonConvert.DeserializeObject<User>(msg.ToString());

            return Ok(new { usuario });
        }

        [HttpGet]
        [Route("system/users/{IdUsuario}")]
        public IHttpActionResult GetUsuario(HttpRequestMessage request)
        {

            
            var re = Request;
            var headers = re.Headers;
            var IdUsuario = re.RequestUri.Segments[4];

            if (IdUsuario != null)
            {
                var cnndf = setCnnDF();
                var mng = new ManagerPerfiles(logger, cnndf);
                string msg = mng.GetUsuario(IdUsuario);
                User usuario = JsonConvert.DeserializeObject<User>(msg.ToString());

                return Ok(new { usuario });
            }
            return InternalServerError(new Exception("Vuelva a entrar al Sistema"));
        }


        [HttpGet]
        [Route("system/users/accesos/{idgrupo}")]
        public IHttpActionResult GetAccesos(HttpRequestMessage request)
        {
            var re = Request;
            var headers = re.Headers;
            var idgrupo = re.RequestUri.Segments[5];

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            string msg = mng.GetAccesosGrupo(idgrupo);
            List<spSystem_UsAccesos_Result> accesos = JsonConvert.DeserializeObject<List<spSystem_UsAccesos_Result>>(msg.ToString());

            return Ok(accesos);
        }

        [HttpGet]
        [Route("system/users/accesos/menu")]
        public IHttpActionResult GetConfigMenu(HttpRequestMessage request)
        {
            var re = Request;
            var headers = re.Headers;
            
            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            string msg = mng.GetConfigMenu();
            List<spSystem_ConfigMenu_Result> menu = JsonConvert.DeserializeObject<List<spSystem_ConfigMenu_Result>>(msg.ToString());

            return Ok(menu);
        }


        [HttpGet]
        [Route("system/grupos")]
        public IHttpActionResult GetGrupos(HttpRequestMessage request)
        {
            var re = Request;
            var headers = re.Headers;
            //var idgrupo = re.RequestUri.Segments[5];

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            string msg = mng.GetGrupoList();
            List<OGRP> accesos = JsonConvert.DeserializeObject<List<OGRP>>(msg.ToString());

            return Ok(accesos);
        }


        [HttpPost]
        [Route("system/grupos/addacceso")]
        public IHttpActionResult AddGrupoAcceso(HttpRequestMessage request)
        {
            var jsonRequest = request.Content.ReadAsStringAsync().Result;
            JObject json;
            try
            {
                //logger.Info("{0}", jsonRequest.ToString());
                json = JObject.Parse(jsonRequest);
            }
            catch
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Json de entrada Incorrecto"));
            }

            var re = Request;
            var headers = re.Headers;
            //var idgrupo = re.RequestUri.Segments[5];

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            var item = JsonConvert.DeserializeObject<GRP1>(json.ToString());
            mng.AddGrupoAcceso(item);
            //string msg = mng.GetGrupoList();


            return Ok();
        }

        [HttpPost]
        [Route("system/login")]
        public IHttpActionResult LoginUser(HttpRequestMessage request)
        {
            var jsonRequest = request.Content.ReadAsStringAsync().Result;
            JObject json;
            try
            {
                //logger.Info("{0}", jsonRequest.ToString());
                json = JObject.Parse(jsonRequest);
            }
            catch
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Json de entrada Incorrecto"));
            }
            
            Login lg = JsonConvert.DeserializeObject<Login>(json.ToString());
            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            string msg = mng.Login(lg.usuario, lg.password);
            LoginOut login = JsonConvert.DeserializeObject<LoginOut>(msg.ToString());
            return Ok(new { login });

        }

        [HttpGet]
        [Route("system/login/{IdUsuario}")]
        public IHttpActionResult GetUsuario_Login(HttpRequestMessage request)
        {
            var re = Request;
            var headers = re.Headers;
            var IdUsuario = re.RequestUri.Segments[4];

            var cnndf = setCnnDF();
            var mng = new ManagerPerfiles(logger, cnndf);
            string msg = mng.GetUsuario_Login(IdUsuario);
            User usuario = JsonConvert.DeserializeObject<User>(msg.ToString());

            return Ok(new { usuario });
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
    }
}
