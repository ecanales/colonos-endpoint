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
    public class ManagerAnimal
    {
        Logger logger;
        public ManagerAnimal(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_ITM5 repo = new Repo_ITM5();
                var itm5 = JsonConvert.DeserializeObject<ITM5>(item);
                var json = repo.Add(itm5);
                itm5 = JsonConvert.DeserializeObject<ITM5>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Animal";
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
                Repo_ITM5 repo = new Repo_ITM5();
                var itm5 = JsonConvert.DeserializeObject<ITM5>(item);
                var json = repo.Modify(itm5);
                itm5 = JsonConvert.DeserializeObject<ITM5>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Animal";
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

        public MensajeReturn Delete(string item)
        {
            try
            {
                Repo_ITM5 repo = new Repo_ITM5();
                var itm5 = JsonConvert.DeserializeObject<ITM5>(item);
                repo.Delete(itm5);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.NoContent;
                msg.error = false;
                msg.msg = "Animal";
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

        public MensajeReturn Get(int animalcode)
        {
            try
            {
                Repo_ITM5 repo = new Repo_ITM5();

                var json = repo.Get(animalcode);
                var item = JsonConvert.DeserializeObject<ITM5>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Animal";
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

        public MensajeReturn List()
        {
            try
            {
                Repo_ITM5 repo = new Repo_ITM5();

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<ITM5>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Listado Animal";
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
    }
}
