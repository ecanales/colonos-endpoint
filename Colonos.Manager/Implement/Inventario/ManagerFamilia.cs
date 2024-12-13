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
    public class ManagerFamilia
    {
        Logger logger;
        public ManagerFamilia(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_ITM4 repo = new Repo_ITM4();
                var itm4 = JsonConvert.DeserializeObject<ITM4>(item);
                if(itm4.Correlativo == null)
                    itm4.Correlativo = 21001;
                if (itm4.Correlativo.ToString().Length != 5)
                    itm4.Correlativo = 21001;

                var json = repo.Add(itm4);
                itm4 = JsonConvert.DeserializeObject<ITM4>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Familia";
                msg.data = itm4;
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
                Repo_ITM4 repo = new Repo_ITM4();
                var itm4 = JsonConvert.DeserializeObject<ITM4>(item);
                var json = repo.Get(itm4.FamiliaCode);
                var itm4curr = JsonConvert.DeserializeObject<ITM4>(json);
                itm4.Correlativo = itm4curr.Correlativo;

                json = repo.Modify(itm4);
                itm4 = JsonConvert.DeserializeObject<ITM4>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Familia";
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
                Repo_ITM4 repo = new Repo_ITM4();
                var itm4 = JsonConvert.DeserializeObject<ITM4>(item);
                repo.Delete(itm4);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.NoContent;
                msg.error = false;
                msg.msg = "Familia";
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
                Repo_ITM4 repo = new Repo_ITM4();

                var json = repo.Get(animalcode);
                var item = JsonConvert.DeserializeObject<ITM4>(json);
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
                Repo_ITM4 repo = new Repo_ITM4();

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<ITM4>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count;
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

        public MensajeReturn List(string palabras)
        {
            try
            {
                Repo_ITM4 repo = new Repo_ITM4();

                var json = repo.List(palabras);
                var list = JsonConvert.DeserializeObject<List<ITM4>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count;
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
