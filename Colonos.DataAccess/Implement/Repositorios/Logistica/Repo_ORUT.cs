using Colonos.Entidades;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ORUT
    {
        Logger logger;
        public Repo_ORUT(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(DocumentoRuta item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.ORUT where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var doc = JsonConvert.DeserializeObject<ORUT>(JsonConvert.SerializeObject(item));

                    db.ORUT.Add(doc);
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
                var query = from e in db.ORUT where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<DocumentoRuta>(JsonConvert.SerializeObject(item));

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Get(string scenario_token)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ORUT where e.scenario_token == scenario_token select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<DocumentoRuta>(JsonConvert.SerializeObject(item));

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }
        public string Modify(ORUT item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ORUT.Find(item.DocEntry);
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
                var query = from e in db.spORUT_Listar(estado) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
