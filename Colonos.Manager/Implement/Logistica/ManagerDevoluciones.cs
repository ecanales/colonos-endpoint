using Colonos.DataAccess;
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
    public class ManagerDevoluciones
    {
        Logger logger;
        public ManagerDevoluciones(Logger _logger)
        {
            logger = _logger;

        }

        public MensajeReturn ListMotivos()
        {
            Repo_DEV2 repo = new Repo_DEV2();
            var json = repo.List();
            var doc = JsonConvert.DeserializeObject<List<DEV2>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = doc.Count();
            msg.msg = doc == null ? "Error" : "Ok";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {


                Repo_ODEV repo = new Repo_ODEV(logger);
                var json = repo.Add(doc);
                var docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Ruta";
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
            Repo_ODEV repo = new Repo_ODEV(logger);
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

        public MensajeReturn Modify(string item)
        {

            try
            {
                Documento doc = JsonConvert.DeserializeObject<Documento>(item);

                Repo_ODEV repo = new Repo_ODEV(logger);
                Repo_DEV1 repolin = new Repo_DEV1();
                var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var linupdate = doc.Lineas;

                UpdateDetalle(linupdate, linoriginal);
                var odev = JsonConvert.DeserializeObject<ODEV>(item);

                if (odev.Custodio != null && odev.Custodio.Length > 0)
                {
                    //generar RCT o RCC ??

                }
                var json = repo.Modify(odev);
                var ped = JsonConvert.DeserializeObject<Documento>(json);
                ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));

                if (odev.DocEstado == "C")
                {
                   

                }
                

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Devolución no existe" : "Actualizar Devolución";
                msg.data = ped;

                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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

        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_DEV1 repo = new Repo_DEV1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<DEV1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<DEV1>(json);
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
                            var lin = JsonConvert.DeserializeObject<DEV1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<DEV1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<DEV1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
    }
}
