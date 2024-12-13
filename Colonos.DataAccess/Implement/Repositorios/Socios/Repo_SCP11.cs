using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP11
    {
        public void Add(SCP11_tmp item)
        {
            using (var db = new cnnDatos())
            {
                    db.SCP11_tmp.Add(item);
                    db.SaveChanges();
            }
        }

        public void ExecuteSql(string cmd)
        {
            using (var db = new cnnDatos())
            {
                db.Database.ExecuteSqlCommand(cmd);
                db.SaveChanges();
            }
        }
    }
}
