using Colonos.Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OTRX
    {
        public string Add(Transaccion item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OTRX where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var otrx = JsonConvert.DeserializeObject<OTRX>(JsonConvert.SerializeObject(item));

                    db.OTRX.Add(otrx);
                    db.SaveChanges();

                    int docentry = otrx.DocEntry;
                    item.DocEntry = docentry;

                    Repo_TRX1 repo = new Repo_TRX1();

                    foreach (var i in item.Lineas)
                    {
                        i.DocEntry = docentry;
                        repo.Add(new TRX1
                        {
                            DocEntry = i.DocEntry,
                            DocLinea = i.DocLinea,
                            CantidadSolicitada = i.CantidadSolicitada,
                            Costo=i.Costo,
                            ProdCode=i.ProdCode,
                            ProdNombre = i.ProdNombre,
                            MedidaCode=i.MedidaCode,
                        });
                    }
                    return Get(docentry);
                }
            }
            return JSONresult;
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OTRX where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Transaccion>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_TRX1 repo = new Repo_TRX1();
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<TransaccionLinea>>(lineas);
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Modify(OTRX item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OTRX.Find(item.DocEntry);
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

        public bool Delete(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OTRX.Find(docentry);
                if (t != null)
                {
                    db.OTRX.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OTRX select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
