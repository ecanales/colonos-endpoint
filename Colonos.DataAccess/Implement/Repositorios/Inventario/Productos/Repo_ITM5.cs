using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM5
    {
        public string Add(ITM5 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM5.Add(item);
                db.SaveChanges();
                var JSONresult = JsonConvert.SerializeObject(item);
                return JSONresult;
            }
        }

        public string Get(int id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM5 where e.AnimalCode == id select e;
                var result = query.FirstOrDefault();
                if (result != null)
                {
                    result.FactorPrecio = result.FactorPrecio ?? 0;
                    result.Volumen = result.Volumen ?? 0;
                    result.DescVolumen = result.DescVolumen ?? 0;
                    result.Margen = result.Margen ?? 0;
                }
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM5 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM5.Find(item.AnimalCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }
                string JSONresult = JsonConvert.SerializeObject(item);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;

            }
        }

        public void Delete(ITM5 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM5.Find(item.AnimalCode);
                if (t != null)
                {
                    db.ITM5.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM5 orderby e.AnimalNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
