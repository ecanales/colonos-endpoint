using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_PED1
    {
        Logger logger;
        public Repo_PED1(Logger _logger)
        {
            logger = _logger;
        }
        public string Add(PED1 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.PED1 where e.DocLinea == item.DocLinea && e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    
                    //actualizar asignado 
                    Repo_OITB repo = new Repo_OITB();
                    var json= repo.Get(item.ProdCode, item.BodegaCode);
                    var oitb = JsonConvert.DeserializeObject<OITB>(json);
                    if(oitb!=null)
                    {
                        
                        oitb.Asignado = (oitb.Asignado ?? 0) + (item.CantidadSolicitada - item.SolicitadoAnterior);
                        repo.Modify(oitb);

                        item.SolicitadoAnterior = item.CantidadSolicitada;
                        db.PED1.Add(item);
                    }
                    db.SaveChanges();

                    JSONresult = JsonConvert.SerializeObject(item, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
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
                var query = from e in db.PED1 where e.DocLinea == doclinea select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(PED1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PED1.Find(item.DocEntry, item.DocLinea);
                if (t != null)
                {
                    //Actualizar Asignado
                    Repo_OITB repo = new Repo_OITB();
                    var json = repo.Get(item.ProdCode, item.BodegaCode);
                    var oitb = JsonConvert.DeserializeObject<OITB>(json);
                    if (oitb != null)
                    {
                        oitb.Asignado = (oitb.Asignado ?? 0) + (item.CantidadSolicitada - item.SolicitadoAnterior);
                        repo.Modify(oitb);
                        item.SolicitadoAnterior = item.CantidadSolicitada;
                    }

                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();

                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public bool Delete(PED1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PED1.Find(item.DocEntry, item.DocLinea);
                if (t != null)
                {
                    //Actualizar Asignado
                    Repo_OITB repo = new Repo_OITB();
                    var json = repo.Get(item.ProdCode, item.BodegaCode);
                    var oitb = JsonConvert.DeserializeObject<OITB>(json);
                    if (oitb != null)
                    {
                        oitb.Asignado = oitb.Asignado ?? 0 - item.CantidadSolicitada;
                        repo.Modify(oitb);
                    }

                    db.PED1.Remove(t);
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
                var t = from e in db.PED1 where e.DocEntry== docentry select e;
                foreach(var l in t)
                {
                    db.PED1.Remove(l);
                    db.SaveChanges();
                }
                return true;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PED1 select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return JSONresult;
            }
        }

        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PED1 where e.DocEntry==docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return JSONresult;
            }
        }
    }
}
