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
    public class ManagerDirecciones
    {
        Logger logger;
        public ManagerDirecciones(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_SCP1 repo = new Repo_SCP1(logger);

                var scp1 = JsonConvert.DeserializeObject<SCP1>(item);
                var json = repo.Add(scp1);
                var list = JsonConvert.DeserializeObject<SCP1>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Direccion";
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

        public MensajeReturn Get(int direccioncode)
        {
            try
            {
                Repo_SCP1 repo = new Repo_SCP1(logger);

                var json = repo.Get(direccioncode);
                var item = JsonConvert.DeserializeObject<SCP1>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = item == null ? "Direccion no existe" : "Direccion";
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
                Repo_SCP1 repo = new Repo_SCP1(logger);

                var scp1 = JsonConvert.DeserializeObject<SCP1>(item);
                var json = repo.Modify(scp1);
                var contacto = JsonConvert.DeserializeObject<SCP1>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = contacto.SocioCode == null ? "Direccion no existe" : "Actualizar Direccion";
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

        public MensajeReturn Delete(string item)
        {
            try
            {
                Repo_SCP1 repo = new Repo_SCP1(logger);
                var scp1 = JsonConvert.DeserializeObject<SCP1>(item);
                var dir = repo.Delete(scp1);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = dir ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = !dir;
                msg.msg = !dir ? "Direccion no existe" : "Direccion";
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

        public MensajeReturn Delete(int direccioncode)
        {
            try
            {
                Repo_SCP1 repo = new Repo_SCP1(logger);
                var dir = repo.Delete(direccioncode);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = dir ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = !dir;
                msg.msg = !dir ? "Direccion no existe" : "Direccion";
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
                Repo_SCP1 repo = new Repo_SCP1(logger);

                var json = repo.List(sociocode);
                var list = JsonConvert.DeserializeObject<List<SCP1>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Direcciones Socio";
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

        public void UpdateDirecciones(List<SCP1> ItemsUpdate, List<SCP1> ItemsCurr)
        {
            Repo_SCP1 repo = new Repo_SCP1(logger);
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (SCP1 ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<SCP1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SCP1>(json);
                        repo.Delete(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<SCP1> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        SCP1 cd = ItemsUpdate.Find(x => x.DireccionCode == i.DireccionCode);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<SCP1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<SCP1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<SCP1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }

        


        public MensajeReturn Propiedades()
        {
            try
            {

                Repo_OCOM repoco = new Repo_OCOM();
                Repo_OCIU repoci = new Repo_OCIU();
                Repo_OREG reporg = new Repo_OREG();
                

                var prop = new DireccionPropiedades();

                var json = repoco.List(); prop.comunas= JsonConvert.DeserializeObject<List<OCOM>>(json);
                json = repoci.List(); prop.ciudades = JsonConvert.DeserializeObject<List<OCIU>>(json);
                json = reporg.List(); prop.regiones = JsonConvert.DeserializeObject<List<OREG>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = prop.comunas.Count +
                    prop.ciudades.Count +
                    prop.regiones.Count;
                msg.msg = "Propiedades Direcciones";
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
