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
    public class ManagerContactos
    {

        Logger logger;
        public ManagerContactos(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_SCP2 repo = new Repo_SCP2(logger);

                var scp2 = JsonConvert.DeserializeObject<SCP2>(item);
                var json = repo.Add(scp2);
                var list = JsonConvert.DeserializeObject<SCP2>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Contacto";
                msg.data = list;
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

        public MensajeReturn Get(int contactocode)
        {
            try
            {
                Repo_SCP2 repo = new Repo_SCP2(logger);

                var json = repo.Get(contactocode);
                var item = JsonConvert.DeserializeObject<SCP2>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = item == null ? "Contacto no existe" : "Contacto";
                msg.data = item;
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

        public MensajeReturn Modify(string item)
        {
            try
            {
                Repo_SCP2 repo = new Repo_SCP2(logger);

                var scp2 = JsonConvert.DeserializeObject<SCP2>(item);
                var json = repo.Modify(scp2);
                var contacto = JsonConvert.DeserializeObject<SCP2>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = contacto.SocioCode == null ? "Contacto no existe" : "Actualizar Contacto";
                msg.data = contacto;
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

        public MensajeReturn Delete(int conactocode)
        {
            try
            {
                Repo_SCP2 repo = new Repo_SCP2(logger);

                var item = repo.Delete(conactocode);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = item ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = !item;
                msg.msg = !item ? "Contacto no existe" : "Contacto";
                msg.data = "";
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

        public MensajeReturn List(string sociocode)
        {
            try
            {
                Repo_SCP2 repo = new Repo_SCP2(logger);

                var json = repo.List(sociocode);
                var list = JsonConvert.DeserializeObject<List<SCP2>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Contactos Socio";
                msg.data = list;
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

        public void UpdateContactos(List<SCP2> ItemsUpdate, List<SCP2> ItemsCurr)
        {
            Repo_SCP2 repo = new Repo_SCP2(logger);
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (SCP2 ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<SCP2>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SCP2>(json);
                        repo.Delete(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<SCP2> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        SCP2 cd = ItemsUpdate.Find(x => x.ContactoCode == i.ContactoCode);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<SCP2>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<SCP2>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SCP2>(json);
                        repo.Add(lin);
                    }
                }
            }
        }

        public MensajeReturn Propiedades()
        {
            try
            {

                Repo_SCP8 repo8 = new Repo_SCP8();

                var prop = new ContactoPropiedades();

                var json = repo8.List(); prop.tipocontacto = JsonConvert.DeserializeObject<List<SCP8>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = prop.tipocontacto.Count;
                msg.msg = "Propiedades Contactos";
                msg.data = prop;
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
    }
}
