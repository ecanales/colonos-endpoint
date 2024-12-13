using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_OPER
    {
        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OPER orderby e.Nombre select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
