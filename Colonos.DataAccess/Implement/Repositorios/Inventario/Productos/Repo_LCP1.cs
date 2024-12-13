using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_LCP1
    {
        public void Add(LPC1 item)
        {
            using (var db = new cnnDatos())
            {
                db.LPC1.Add(item);
                db.SaveChanges();

            }
        }

        public string Get(string listacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.LPC1 where e.ListaCode == listacode select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(LPC1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.LPC1.Find(item.ProdCode);
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

        public void Delete(LPC1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.LPC1.Find(item.ProdCode);
                if (t != null)
                {
                    db.LPC1.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.LPC1 orderby e.ProdCode select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string listcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.LPC1 orderby e.ProdCode where e.ListaCode==listcode select e;
                var result = query.ToList();
                
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
