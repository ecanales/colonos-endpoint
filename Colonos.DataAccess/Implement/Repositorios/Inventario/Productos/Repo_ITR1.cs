using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITR1
    {
        public void Add(ITR1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.ITR1 where e.ProdCode == item.ProdCode && e.ProdCodeRef == item.ProdCodeRef select e;
                if (t.FirstOrDefault() == null)
                {
                    db.ITR1.Add(item);
                    db.SaveChanges();
                }
            }
        }

        public string Get(string prodcode, string prodcoderef)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITR1 where e.ProdCode == prodcode && e.ProdCodeRef==prodcoderef select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITR1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITR1.Find(item.ProdCode,item.ProdCodeRef);
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

        public void Delete(string prodcode, string prodcoderef)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITR1.Find(prodcode,  prodcoderef);
                if (t != null)
                {
                    db.ITR1.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITR1 select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string prodcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITR1 where e.ProdCode== prodcode select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string prodcode,string bodegacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spReceta_Stock(prodcode,bodegacode) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
