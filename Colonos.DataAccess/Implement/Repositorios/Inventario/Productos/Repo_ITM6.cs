using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM6
    {
        public void Add(ITM6 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM6.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(int id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM6 where e.FormatoCode == id select e;
                var result = query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM6 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM6.Find(item.FormatoCode);
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

        public void Delete(ITM6 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM6.Find(item.FormatoCode);
                if (t != null)
                {
                    db.ITM6.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM6 orderby e.FormatoNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
