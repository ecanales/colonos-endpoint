using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.Entidades;
using Colonos.Entidades.Defontana;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Manager
{
    public class ManagerParametros
    {
        Logger logger;
        public ManagerParametros(Logger _logger)
        {
            logger = _logger;
        }
        public MensajeReturn Get()
        {
            MensajeReturn msg;
            try
            {
                Repo_OCFG_VTA repo = new Repo_OCFG_VTA();
                var json = repo.Get(1);
                var param = JsonConvert.DeserializeObject<OCFG_VTA>(json);

                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Parámetros";
                msg.data = param;
                return msg;
            }
            catch(Exception ex)
            {
                logger.Error("Error parametros generales, mensaje: {0}", ex.Message);
                logger.Error("Error parametros generales, {0}", ex.StackTrace);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.BadRequest;
                msg.count = 1;
                msg.error = true;
                msg.msg = ex.Message;
                msg.data = null;
                return msg;
            }
        }

        public MensajeReturn Modify(string item)
        {
            MensajeReturn msg;
            try
            {
                var ocfg = JsonConvert.DeserializeObject<OCFG_VTA>(item);

                Repo_OCFG_VTA repo = new Repo_OCFG_VTA();
                var json = repo.Modify(ocfg);
                var param = JsonConvert.DeserializeObject<OCFG_VTA>(json);

                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Parámetros";
                msg.data = param;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error("Error parametros generales, mensaje: {0}", ex.Message);
                logger.Error("Error parametros generales, {0}", ex.StackTrace);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.BadRequest;
                msg.count = 1;
                msg.error = true;
                msg.msg = ex.Message;
                msg.data = null;
                return msg;
            }
        }
    }
}
