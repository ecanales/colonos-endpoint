using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP12
    {
        public string Get(string code)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP12 where e.code == code select e;
                var result = query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(result);

                return JSONresult;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP12 select e;
                var result = query.ToList();

                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
