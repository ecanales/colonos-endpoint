using Colonos.DataAccess.Repositorios;
using Colonos.Entidades;
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
    public class ManagerRendicion
    {
        Logger logger;
        public ManagerRendicion(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;

            Repo_OREN repo = new Repo_OREN(logger);

            repo.Add(doc.UsuarioCode);

            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.count = 1;
            msg.error = false;
            msg.msg = "Rendición Producción";
            msg.data = "";
            return msg;
        }

        public MensajeReturn Get(int docentry)
        {
            Repo_OREN repo = new Repo_OREN(logger);
            //Repo_PED1 repolin = new Repo_PED1(logger);

            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Rendicion no existe" : "Rendicion Produccion";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn List(string estado)
        {
            Repo_OREN repo = new Repo_OREN(logger);
            var json = repo.List(estado);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Rendicion Produccion";
            msg.data = list;
            return msg;
        }

    }
}
