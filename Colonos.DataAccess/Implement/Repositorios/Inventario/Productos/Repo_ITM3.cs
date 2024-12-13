using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM3
    {
        public void Add(ITM3 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM3.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(string id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM3 where e.TipoCode == id select e;
                var result = query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM3 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM3.Find(item.TipoCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }
                string JSONresult = JsonConvert.SerializeObject(item);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;

            }
        }

        public void Delete(ITM3 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM3.Find(item.TipoCode);
                if (t != null)
                {
                    db.ITM3.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM3 orderby e.TipoNombre select e;
                var result= query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
