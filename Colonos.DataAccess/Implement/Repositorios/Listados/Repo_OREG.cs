using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OREG
    {
        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OREG orderby e.RegionNombre select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
