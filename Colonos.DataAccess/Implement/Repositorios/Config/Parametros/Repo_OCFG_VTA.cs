using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OCFG_VTA
    {
        public void Add(OCFG_VTA item)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.OCFG_VTA where e.ParamCode == item.ParamCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OCFG_VTA.Add(item);
                    db.SaveChanges();
                }
            }
        }

        public string Get(int paramcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OCFG_VTA where e.ParamCode == paramcode select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                result.FactorPrecio = result.FactorPrecio ?? 0;
                result.Volumen = result.Volumen ?? 0;
                result.DescVolumen = result.DescVolumen ?? 0;
                result.Margen = result.Margen ?? 0;
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OCFG_VTA item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OCFG_VTA.Find(item.ParamCode);
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
    }
}
