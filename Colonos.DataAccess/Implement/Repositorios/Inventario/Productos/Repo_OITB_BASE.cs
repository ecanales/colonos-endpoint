using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OITB_BASE
    {
        public string Add(OITB_BASE item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OITB_BASE where e.ProdCode == item.ProdCode && e.BodegaCode == item.BodegaCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OITB_BASE.Add(item);
                    db.SaveChanges();
                    //crear registro en todas las bodegas


                    JSONresult = JsonConvert.SerializeObject(item);
                    //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                    return JSONresult;
                }
                else
                {
                    this.Modify(item);
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(string prodcode, string bodegacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OITB_BASE where e.ProdCode == prodcode && e.BodegaCode == bodegacode select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string List(string bodegacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OITB_BASE where e.BodegaCode == bodegacode select e;
                var result = query.ToList();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
        public string Modify(OITB_BASE item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OITB_BASE.Find(item.ProdCode, item.BodegaCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();

                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
    }
}
