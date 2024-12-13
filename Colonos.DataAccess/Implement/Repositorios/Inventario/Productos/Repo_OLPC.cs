using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OLPC
    {
        public void Add(OLPC item)
        {
            using (var db = new cnnDatos())
            {
                db.OLPC.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(string listacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OLPC where e.ListaCode == listacode select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OLPC item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OLPC.Find(item.ListaCode);
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

        public void Delete(OLPC item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OLPC.Find(item.ListaCode);
                if (t != null)
                {
                    db.OLPC.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public void Delete(string listacode)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OLPC.Find(listacode);
                if (t != null)
                {
                    db.OLPC.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OLPC orderby e.Desde select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
