using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_OBAN
    {
        public string Get(string bandejacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBAN where e.BendejaCode==bandejacode select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBAN select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
