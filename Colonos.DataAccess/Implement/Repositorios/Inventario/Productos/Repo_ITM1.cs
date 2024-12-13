using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM1
    {
        public void Add(ITM1 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM1.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(string id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM1 where e.MedidaCode == id select e;
                var result= query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM1.Find(item.MedidaCode);
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

        public void Delete(ITM1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM1.Find(item.MedidaCode);
                if (t != null)
                {
                    db.ITM1.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM1 orderby e.MedidaNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
