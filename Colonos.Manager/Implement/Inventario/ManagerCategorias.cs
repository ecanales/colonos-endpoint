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
    public class ManagerCategorias
    {
        Logger logger;
        public ManagerCategorias(Logger _logger)
        {
            logger = _logger;
        }
        public MensajeReturn ListarCategorias()
        {
            try
            {
                Repo_ITM2 repo = new Repo_ITM2();

                var json = repo.List();
                var list= JsonConvert.DeserializeObject<List<Categorias>>(json);
                list.Add(new Categorias { CategoriaCode = "", CategoriaNombre = "", Orden = 0 });
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Listado Categorías";
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
