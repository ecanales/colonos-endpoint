using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_JsonClientes
    {
        public void Add(ClienteDF item)
        {
            using (var db = new cnnDatos())
            {
                var t=(from e in db.ClienteDF where e.fileID==item.fileID select e).FirstOrDefault();
                if (t == null)
                {
                    db.ClienteDF.Add(item);
                    db.SaveChanges();
                }
            }
        }

        //public List<JsonClientes> List()
        //{
        //    using (var db = new cnnDatos())
        //    {
        //        var query = from e in db.JsonClientes select e;
        //        return query.ToList();

        //    }
        //}
    }
}
