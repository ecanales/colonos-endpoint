using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_OITB
    {
        public string Add(OITB item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OITB where e.ProdCode == item.ProdCode && e.BodegaCode==item.BodegaCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OITB.Add(item);
                    db.SaveChanges();
                    //crear registro en todas las bodegas


                    JSONresult = JsonConvert.SerializeObject(item);
                    //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                    return JSONresult;
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
                var query = from e in db.OITB where e.ProdCode == prodcode && e.BodegaCode == bodegacode select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OITB item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OITB.Find(item.ProdCode, item.BodegaCode);
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
