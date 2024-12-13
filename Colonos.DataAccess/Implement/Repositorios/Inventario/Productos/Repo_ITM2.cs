using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM2
    {
        public void Add(ITM2 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM2.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(string id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM2 where e.CategoriaCode == id select e;
                var result = query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM2 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM2.Find(item.CategoriaCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public void Delete(ITM2 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM2.Find(item.CategoriaCode);
                if (t != null)
                {
                    db.ITM2.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM2 orderby e.Orden select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
