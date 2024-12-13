using Colonos.DataAccess;
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
    public class ManagerSolicitudMP
    {
        Logger logger;

        public ManagerSolicitudMP(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {
                Repo_OSMP repo = new Repo_OSMP(logger);
                var json = repo.Add(doc);
                var docgenerado = JsonConvert.DeserializeObject<Documento>(json);

                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Solicitud de Mataria Prima Generado";
                msg.data = docgenerado;
                return msg;
            }
            catch (Exception ex)
            {
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.InternalServerError;
                msg.error = true;
                msg.msg = ex.Message;
                msg.data = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    msg.data += JsonConvert.SerializeObject(ex);
                }

                return msg;
            }
        }

        public MensajeReturn Get(int docentry)
        {
            Repo_OSMP repo = new Repo_OSMP(logger);

            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Solicitud no existe" : "Solicitud";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn Modify(string item)
        {

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);

            Repo_OSMP repo = new Repo_OSMP(logger);
            Repo_SMP1 repolin = new Repo_SMP1();
            var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            var linupdate = doc.Lineas;

            UpdateDetalle(linupdate, linoriginal);
            var oped = JsonConvert.DeserializeObject<OSMP>(item);

            var json = repo.Modify(oped);
            var ped = JsonConvert.DeserializeObject<Documento>(json);
            ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.msg = ped == null ? "Solicitud no existe" : "Actualizar Solicitud";
            msg.data = ped;

            return msg;
        }

        public MensajeReturn List(string bodegacode, string estado)
        {
            Repo_OSMP repo = new Repo_OSMP(logger);
            var json = repo.List(estado);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Solicitudes MP";
            msg.data = list;
            return msg;
        }

        public MensajeReturn Search(string palabras, string vendedocode)
        {
            Repo_OSMP repo = new Repo_OSMP(logger);

            var json = repo.Search(palabras, vendedocode);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Solicitudes";
            msg.data = list;
            return msg;
        }

        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_SMP1 repo = new Repo_SMP1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<SMP1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SMP1>(json);
                        repo.Delete(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<DocumentoLinea> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        DocumentoLinea cd = ItemsUpdate.Find(x => x.DocLinea == i.DocLinea);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<SMP1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<SMP1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SMP1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
    }
}
