using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM7
    {
        public void Add(ITM7 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM7.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(int id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM7 where e.RefrigeraCode == id select e;
                var result =  query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM7 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM7.Find(item.RefrigeraCode);
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

        public void Delete(ITM7 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM7.Find(item.RefrigeraCode);
                if (t != null)
                {
                    db.ITM7.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM7 orderby e.RefrigeraNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
