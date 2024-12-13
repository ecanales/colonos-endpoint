using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OUSR
    {
        public string Login(string us, string pw)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spLogin(us, pw) select e;

                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(query.FirstOrDefault(), Formatting.None);

                return JSONresult;
            }
        }

        public OUSR Add(OUSR item)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.OUSR where e.Usuario == item.Usuario select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OUSR.Add(item);
                    db.SaveChanges();
                    return item;
                }
                return item;
            }
        }

        public string Get(string usuario)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OUSR where e.Usuario == usuario select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Modify(OUSR item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OUSR.Find(item.Usuario);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }

                string JSONresult = JsonConvert.SerializeObject(item);
                JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public void Delete(string usuario)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OUSR.Find(usuario);
                if (t != null)
                {
                    db.OUSR.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OUSR select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string GetUsuario_Login(string IdUsuario)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spSystem_Usuario(IdUsuario) select e;
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(query.FirstOrDefault(), Formatting.None);

                return JSONresult;
            }
        }
    }
}
