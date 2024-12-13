using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_PKG1
    {
        public string Add(PKG1 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.PKG1 where e.DocLinea == item.DocLinea && e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    db.PKG1.Add(item);
                    db.SaveChanges();

                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(int doclinea)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG1 where e.DocLinea == doclinea select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(PKG1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PKG1.Find(item.DocEntry, item.DocLinea);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public bool Delete(PKG1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PKG1.Find(item.DocEntry, item.DocLinea);
                if (t != null)
                {
                    db.PKG1.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.PKG1 where e.DocEntry == docentry select e;
                foreach (var l in t)
                {
                    db.PKG1.Remove(l);
                    db.SaveChanges();
                }
                return true;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG1 select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string PickingDetalle(int docentry, string origen)
        {
            using (var db = new cnnDatos())
            {
                string JSONresult = "";
                if (origen == "produccion")
                {
                    var query = from e in db.spPicking_Produccion_Detalle(docentry) select e;
                    var result = query.ToList();
                    JSONresult = JsonConvert.SerializeObject(result);
                }
                if (origen == "toledo")
                {
                    var query = from e in db.spPicking_Toledo_Detalle(docentry) select e;
                    var result = query.ToList();
                    JSONresult = JsonConvert.SerializeObject(result);
                }
                return JSONresult;
            }
        }

        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG1 where e.DocEntry == docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
