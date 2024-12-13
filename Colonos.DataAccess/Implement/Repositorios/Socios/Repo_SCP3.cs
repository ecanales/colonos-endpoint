using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP3
    {
        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP3 orderby e.Orden select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
        public string Get(int condcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP3 where e.CondicionCode == condcode select e;
                var item = query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(item);
                return JSONresult;
            }
        }
    }
}
