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
    public class Repo_FAC1
    {
        Logger logger;
        public Repo_FAC1(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(FAC1 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.FAC1 where e.DocLinea == item.DocLinea && e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {

                    
                    Repo_OITB repo = new Repo_OITB();
                    Repo_PED1 repopedlin = new Repo_PED1(logger);
                    Repo_OPED repoped = new Repo_OPED(logger);


                    //rebajar el pendiente del pedido -----
                    var json = repoped.Get(Convert.ToInt32(item.BaseEntry));
                    var ped = JsonConvert.DeserializeObject<Documento>(json);
                    var linpedido = ped.Lineas.Find(x => x.DocEntry == item.BaseEntry && x.DocLinea == item.BaseLinea);
                    linpedido.CantidadPendiente -= item.CantidadSolicitada;
                    linpedido.CantidadReal -= item.CantidadSolicitada;
                    linpedido.CantidadEntregada += item.CantidadSolicitada;
                    //si la cantidad pendiente es negativa cerrar el item
                    if (linpedido.CantidadPendiente <= 0)
                    {
                        linpedido.LineaEstado = "C";
                        linpedido.CantidadPendiente = 0;
                    }
                        
                    json = JsonConvert.SerializeObject(linpedido);
                    var ped1 = JsonConvert.DeserializeObject<PED1>(json);
                    repopedlin.Modify(ped1);
                    //-------------------------------------

                    //rebajar asignado y stock ------------
                    json = repo.Get(item.ProdCode, item.BodegaCode);
                    var oitb = JsonConvert.DeserializeObject<OITB>(json);
                    if (oitb != null)
                    {
                        oitb.Asignado = (oitb.Asignado ?? 0) - (item.CantidadSolicitada);
                        oitb.Stock = (oitb.Stock ?? 0) - (item.CantidadSolicitada);
                        if (oitb.Asignado < 0)
                            oitb.Asignado = 0;
                        if (oitb.Stock < 0)
                            oitb.Stock = 0;

                        repo.Modify(oitb);
                        db.FAC1.Add(item);
                    }
                    //-------------------------------------
                    db.SaveChanges();

                    
                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            return JSONresult;
        }

        public string Get(int doclinea)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.FAC1 where e.DocLinea == doclinea select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(FAC1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.FAC1.Find(item.DocEntry, item.DocLinea);
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

        public bool Delete(FAC1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.FAC1.Find(item.DocEntry, item.DocLinea);
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

                    db.FAC1.Remove(t);
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
                var t = from e in db.FAC1 where e.DocEntry == docentry select e;
                foreach (var l in t)
                {
                    db.FAC1.Remove(l);
                    db.SaveChanges();
                }
                return true;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.FAC1 select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.FAC1 where e.DocEntry == docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
