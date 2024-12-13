using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_ITM9
    {
        public void Add(ITM9 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM9.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(int id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM9 where e.MarcaCode == id select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM9 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM9.Find(item.MarcaCode);
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

        public void Delete(ITM9 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM9.Find(item.MarcaCode);
                if (t != null)
                {
                    db.ITM9.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM9 orderby e.MarcaNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
