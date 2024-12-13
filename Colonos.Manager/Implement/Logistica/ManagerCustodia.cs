using Colonos.DataAccess.Repositorios;
using Colonos.Entidades;
using Colonos.Entidades.Drivin;
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
    public class ManagerCustodia
    {
        Logger logger;
        cnnDrivin cnndrivin;
        public ManagerCustodia(Logger _logger, cnnDrivin _cnndrivin)
        {
            logger = _logger;
            cnndrivin = _cnndrivin;
        }

        public MensajeReturn Get(int docentry)
        {
            Repo_OCUS repo = new Repo_OCUS();
            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Ruta no existe" : "Ruta";
            msg.data = doc;

            return msg;
        }
    }
}
