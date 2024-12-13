using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    
    public class Repo_SCP2
    {
        Logger logger;
        public Repo_SCP2(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(SCP2 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.SCP2 where e.SocioCode == item.SocioCode && e.Nombre==item.Nombre && e.ContactoTipo==item.ContactoTipo select e;
                if (t.FirstOrDefault() == null)
                {
                    db.SCP2.Add(item);
                    db.SaveChanges();
                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(int contactocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP2 where e.ContactoCode == contactocode select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(SCP2 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP2.Find(item.ContactoCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();

                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public bool Delete(int contactocode)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP2.Find(contactocode);
                if (t != null)
                {
                    db.SCP2.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(SCP2 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP2.Find(item.ContactoCode);
                if (t != null)
                {
                    db.SCP2.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.SCP2 where e.SocioCode == sociocode select e;
                foreach (var c in t)
                {
                    db.SCP2.Remove(c);
                    db.SaveChanges();
                }
                return true;
            }
        }
        public string List(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP2 where e.SocioCode==sociocode select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
