﻿using Colonos.Entidades;
using Colonos.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Colonos.EndPoint.Controllers
{
    [RoutePrefix("api/socios")]
    public class DireccionesController : ApiController
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile");

        [HttpGet] //get_id
        [Route("{sociocode}/direcciones/{direccioncode}")]
        public IHttpActionResult Get(string sociocode, string direccioncode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var mng = new ManagerDirecciones(logger);
            if (direccioncode == null || direccioncode == "")
                direccioncode = "0";

            var item = mng.Get(Convert.ToInt32(direccioncode));
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
        [Route("{sociocode}/direcciones")]
        public IHttpActionResult Modify(string sociocode)
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

            var mng = new ManagerDirecciones(logger);
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

        [HttpDelete] //delete id
        [Route("{sociocode}/direcciones/{direccioncode}")]
        public IHttpActionResult Delete(string sociocode, string direccioncode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var mng = new ManagerDirecciones(logger);
            if (direccioncode == null || direccioncode == "")
                direccioncode = "0";

            var item = mng.Delete(Convert.ToInt32(direccioncode));
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
        [Route("{sociocode}/direcciones")]
        public IHttpActionResult Add(string sociocode)
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

            var mng = new ManagerDirecciones(logger);
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

        [HttpGet] //list
        [Route("{sociocode}/direcciones")]
        public IHttpActionResult List(string sociocode)
        {
            logger.Info("request {0}", Request.RequestUri);
            var mng = new ManagerDirecciones(logger);
            var item = mng.List(sociocode);
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
        [Route("direcciones/propiedades")]
        public IHttpActionResult propiedades(HttpRequestMessage request)
        {
            logger.Info("request {0}", Request.RequestUri);

            var mng = new ManagerDirecciones(logger);
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
    }
}