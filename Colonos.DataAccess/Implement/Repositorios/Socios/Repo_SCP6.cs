using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP6
    {
        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP6 orderby e.SocioTipoNombre select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
