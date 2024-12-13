using Colonos.Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OCUS
    {
        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OCUS where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var doc = JsonConvert.DeserializeObject<OCUS>(JsonConvert.SerializeObject(item));

                    db.OCUS.Add(doc);
                    db.SaveChanges();

                    int docentry = doc.DocEntry;
                    item.DocEntry = docentry;

                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            return JSONresult;
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OCUS where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Modify(OCUS item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OCUS.Find(item.DocEntry);
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

        public string List(string estado)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OCUS where e.DocEstado==estado select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
