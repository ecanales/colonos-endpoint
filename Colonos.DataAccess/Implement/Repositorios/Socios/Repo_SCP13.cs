using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP13
    {
        public void Add(SCP13 item)
        {
                        
            using (var db = new cnnDatos())
            {
                
                    db.SCP13.Add(item);
                    db.SaveChanges();
            }
        }
        public string List(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP13 where e.SocioCode==sociocode orderby e.IdHistorial descending   select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
