using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_BDA1
    {
        public void Add(BDA1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.BDA1 where e.DocEntry == item.DocEntry && e.BandejaCode==item.BandejaCode && e.DocLinea==item.DocLinea select e;
                if (t.FirstOrDefault() == null)
                {
                    db.BDA1.Add(item);
                    db.SaveChanges();
                }
            }
        }

        public string Get(int docentry, string bandejaCode, int docLinea)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.BDA1 where e.DocEntry == docentry && e.BandejaCode == bandejaCode && e.DocLinea == docLinea select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(BDA1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.BDA1.Find(item.DocEntry,item.BandejaCode,item.DocLinea);
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

        public void Delete(int docentry, string bandejaCode, int docLinea)
        {
            using (var db = new cnnDatos())
            {
                var t = db.BDA1.Find(docentry, bandejaCode, docLinea);
                if (t != null)
                {
                    db.BDA1.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public void Delete(BDA1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.BDA1.Find(item.DocEntry,item.BandejaCode,item.DocLinea);
                if (t != null)
                {
                    db.BDA1.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.BDA1 select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.BDA1 where e.DocEntry== docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string bandejacode, int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.BDA1 where e.DocEntry == docentry && e.BandejaCode== bandejacode select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
