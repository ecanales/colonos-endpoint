using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP10
    {
        public string Add(SCP10 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.SCP10 where e.SocioCode == item.SocioCode && e.Id==item.Id select e;
                if (t.FirstOrDefault() == null)
                {
                    db.SCP10.Add(item);
                    db.SaveChanges();
                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            return JSONresult;
        }

        public string Get(int id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP10 where e.Id == id select e;
                var result = query.FirstOrDefault();
                string JSONresult = JsonConvert.SerializeObject(result);

                return JSONresult;
            }
        }

        public string Modify(SCP10 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP10.Find(item.Id);
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

        public string List(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP10 where e.SocioCode == sociocode select e;
                var result = query.ToList();

                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public bool Delete(int id)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP10.Find(id);
                if (t != null)
                {
                    db.SCP10.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(SCP10 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP10.Find(item.Id);
                if (t != null)
                {
                    db.SCP10.Remove(t);
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
                var t = from e in db.SCP10 where e.SocioCode == sociocode select e;
                foreach (var d in t)
                {
                    db.SCP10.Remove(d);
                    db.SaveChanges();
                }
                return true;
            }
        }
    }
}
