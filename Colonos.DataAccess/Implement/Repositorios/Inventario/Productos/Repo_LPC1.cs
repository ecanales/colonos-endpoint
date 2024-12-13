using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_LPC1
    {
        public void Add(LPC1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.LPC1 where e.ProdCode == item.ProdCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.LPC1.Add(item);
                    db.SaveChanges();
                }
            }
        }

        public string Get(string prodcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.LPC1 where e.ProdCode == prodcode select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(LPC1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = (from e in db.LPC1 where e.ListaCode == item.ListaCode && e.ProdCode == item.ProdCode select e).FirstOrDefault();
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

        //public void Delete(string prodcode)
        //{
        //    using (var db = new cnnDatos())
        //    {
        //        var t = db.LPC1.Find(prodcode);
        //        if (t != null)
        //        {
        //            db.LPC1.Remove(t);
        //            db.SaveChanges();
        //        }

        //    }
        //}

        public void Delete(string prodcode, string listacode)
        {
            using (var db = new cnnDatos())
            {
                var t = (from e in db.LPC1 where e.ListaCode==listacode && e.ProdCode==prodcode select e).FirstOrDefault();
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
                var query = from e in db.LPC1 select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string listacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.LPC1 where e.ListaCode == listacode select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
