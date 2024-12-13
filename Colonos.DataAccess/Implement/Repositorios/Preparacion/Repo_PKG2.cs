using Colonos.Entidades;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_PKG2
    {
        Logger logger;
        public Repo_PKG2(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.PKG2 where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var doc = JsonConvert.DeserializeObject<PKG2>(JsonConvert.SerializeObject(item));

                    db.PKG2.Add(doc);
                    db.SaveChanges();

                    int docentry = doc.DocEntry;
                    item.DocEntry = docentry;

                    Repo_PKG3 repo = new Repo_PKG3();

                    foreach (var i in item.Lineas)
                    {
                        i.DocEntry = docentry;
                        repo.Add(new PKG3
                        {
                            DocEntry = i.DocEntry,
                            DocLinea = i.DocLinea,
                            DocTipo = i.DocTipo,
                            BaseEntry = i.BaseEntry,
                            BaseLinea = i.BaseLinea,
                            BaseTipo = i.BaseTipo,
                            CantidadReal = Convert.ToDecimal(i.CantidadReal),
                            CantidadSolicitada = Convert.ToDecimal(i.CantidadSolicitada),
                            BodegaCode = i.BodegaCode,
                            ProdCode = i.ProdCode,
                            ProdNombre = i.ProdNombre,
                            ProdTipo = i.ProdTipo,
                            ProdCodeRefGlosa = i.ProdCodeRefGlosa,
                            TipoCode = i.TipoCode,
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
                var query = from e in db.PKG2 where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_PKG3 repo = new Repo_PKG3();
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string GetBase(int baseentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG2 where e.BaseEntry == baseentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_PKG3 repo = new Repo_PKG3();
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string GetBase(int baseentry, int baselinea)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG2 where e.BaseEntry == baseentry && e.BaseLinea==baselinea select e;
                var item = query.FirstOrDefault();
                if (item != null)
                {
                    var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                    if (doc != null)
                    {
                        Repo_PKG3 repo = new Repo_PKG3();
                        var lineas = repo.List(item.DocEntry);
                        doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    }

                    string JSONresult = JsonConvert.SerializeObject(doc);

                    return JSONresult;
                }
                return "";
            }
        }

        public string Modify(PKG2 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PKG2.Find(item.DocEntry);
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

        public bool Delete(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PKG2.Find(docentry);
                if (t != null)
                {
                    db.PKG2.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public string List(int baseentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG2 where e.BaseEntry == baseentry select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
