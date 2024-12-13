using Colonos.Entidades;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OREN
    {
        Logger logger;
        public Repo_OREN(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(string usuariocode)
        {
            using (var db = new cnnDatos())
            {
                string cmd = String.Format("dbo.spOREN_GenerarCorte '{0}'", usuariocode);
                db.Database.ExecuteSqlCommand(cmd);
                db.SaveChanges();
            }
            return "";
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OREN where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();
                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_REN2 repo = new Repo_REN2();
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                }
                string JSONresult = JsonConvert.SerializeObject(doc);
                return JSONresult;
            }
        }

        public string List(string estado)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OREN where e.DocEstado==estado select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
