using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OITR
    {
        Logger logger;
        public Repo_OITR(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(OITR item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OITR where e.ProdCode == item.ProdCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OITR.Add(item);
                    db.SaveChanges();
                    JSONresult = JsonConvert.SerializeObject(item);
                    JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(string id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OITR where e.ProdCode == id select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OITR item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OITR.Find(item.ProdCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();

                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public bool Delete(string prodcode)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OITR.Find(prodcode);
                if (t != null)
                {
                    db.OITR.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OITR select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string palabras, string sku)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spReceta_Busqueda(palabras, sku) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
    }
}
