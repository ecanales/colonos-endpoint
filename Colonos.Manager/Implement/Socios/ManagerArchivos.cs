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
    public class ManagerArchivos
    {
        Logger logger;
        public ManagerArchivos(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Addcxc(string item)
        {
            try
            {
                Repo_SCP11 repo = new Repo_SCP11();
                string cmd = "truncate table SCP11_tmp";
                repo.ExecuteSql(cmd);
                var list=JsonConvert.DeserializeObject<List<SCP11_tmp>>(item);
                foreach(var a in list)
                {
                    repo.Add(a);
                }
                cmd = "exec dbo.spSocio_ProcesaArchivoCXC";
                repo.ExecuteSql(cmd);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count;
                msg.msg = "Correcto";
                return msg;
            }
            catch(Exception ex)
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
        
        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_SCP10 repo = new Repo_SCP10();

                var scp10 = JsonConvert.DeserializeObject<SCP10>(item);
                var json = repo.Add(scp10);
                var list = JsonConvert.DeserializeObject<SCP10>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Archivo";
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

        public MensajeReturn Get(int id)
        {
            try
            {
                Repo_SCP2 repo = new Repo_SCP2(logger);

                var json = repo.Get(id);
                var archivo = JsonConvert.DeserializeObject<SCP2>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = archivo == null ? "Archivo no existe" : "Archivo";
                msg.data = archivo;
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
                Repo_SCP10 repo = new Repo_SCP10();

                var scp10 = JsonConvert.DeserializeObject<SCP10>(item);
                var json = repo.Modify(scp10);
                var archivo = JsonConvert.DeserializeObject<SCP10>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = archivo.SocioCode == null ? "Archivo no existe" : "Actualizar Archivo";
                msg.data = archivo;
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

        public MensajeReturn Delete(int id)
        {
            try
            {
                Repo_SCP10 repo = new Repo_SCP10();

                var item = repo.Delete(id);
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
                Repo_SCP10 repo = new Repo_SCP10();

                var json = repo.List(sociocode);
                var list = JsonConvert.DeserializeObject<List<SCP10>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Archivo Socio";
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

        public void UpdateArchivos(List<SCP10> ItemsUpdate, List<SCP10> ItemsCurr)
        {
            Repo_SCP10 repo = new Repo_SCP10();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (SCP10 ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<SCP10>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SCP10>(json);
                        repo.Delete(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<SCP10> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        SCP10 cd = ItemsUpdate.Find(x => x.Id == i.Id);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<SCP10>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<SCP10>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SCP10>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
    }
}
