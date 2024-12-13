using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_REN2
    {
        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.REN2 where e.DocEntry == docentry orderby e.ProdCodeRef,e.Posicion, e.ProdCodeGroup,e.Orden select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
