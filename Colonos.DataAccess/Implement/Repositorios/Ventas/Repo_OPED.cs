using Colonos.Entidades;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OPED
    {
        Logger logger;
        public Repo_OPED(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                Random rnd = new Random();
                var random = rnd.Next(30);
                //using (TransactionScope transaction = new TransactionScope())
                //{
                var t = from e in db.OPED where e.DocEntry == item.DocEntry select e;
                    if (t.FirstOrDefault() == null)
                    {
                        Repo_OITM repoprod = new Repo_OITM(logger);
                        var oped = JsonConvert.DeserializeObject<OPED>(JsonConvert.SerializeObject(item));
                        logger.Info("Input incial OPED.  Random: {0}, DocEntry: {1} {2}", random, oped.DocEntry, JsonConvert.SerializeObject(oped, Formatting.Indented));
                    //**********************************
                    foreach (var i in item.Lineas)
                    {
                        var json = repoprod.Get(i.ProdCode);
                        var prod = JsonConvert.DeserializeObject<OITM>(json);
                        i.FamiliaCode = prod.FamiliaCode;
                        i.AnimalCode = prod.AnimalCode;
                        i.Costo = prod.Costo;

                        var ped1 = JsonConvert.DeserializeObject<PED1>(JsonConvert.SerializeObject(i));
                        ped1.SolicitadoAnterior = ped1.CantidadSolicitada;

                        oped.PED1.Add(ped1);
                        logger.Info("Completando Lineas.  Random: {0}, DocEntry: {1}, ProdCode: {2} {3}", random, ped1.DocEntry, ped1.ProdCode, JsonConvert.SerializeObject(ped1, Formatting.Indented));
                    }
                    logger.Info("Antes de enviar a DB. Random: {0}, DocEntry: {1} {2}", random, oped.DocEntry, JsonConvert.SerializeObject(oped, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                    db.OPED.Add(oped);
                    logger.Info("Despues de enviar a DB. Random: {0}, DocEntry: {1} {2}", random, oped.DocEntry, JsonConvert.SerializeObject(oped, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                    db.SaveChanges();
                    //Actualizar Asignado
                    Repo_OITB repostock = new Repo_OITB();
                    foreach (var i in item.Lineas)
                    {
                        var json = repostock.Get(i.ProdCode, i.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        stock.Asignado += i.CantidadPendiente;
                        repostock.Modify(stock);
                    }
                    logger.Info("SaveChanges enviar a DB. Random: {0}, DocEntry: {1} {2}", random, oped.DocEntry, JsonConvert.SerializeObject(oped, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                    return Get(oped.DocEntry);

                    //**********************************
                }

                
            }
            return JSONresult;
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OPED where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();
               
                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                if (doc != null)
                {
                    Repo_SCP1 repoDir = new Repo_SCP1(logger);
                    Repo_PED1 repo = new Repo_PED1(logger);
                    Repo_OITB repoStock = new Repo_OITB();
                    Repo_ITR1 repoRecea = new Repo_ITR1();
                    var json = "";

                    if (!Convert.ToBoolean(doc.RetiraCliente))
                    {
                        json = repoDir.Get(Convert.ToInt32(doc.DireccionCode));
                        var dir = JsonConvert.DeserializeObject<SCP1>(json);
                        if (dir != null)
                        {
                            var direntrega = String.Format("{0} {1} {2}", dir.Calle ?? "", dir.Numero ?? "", dir.ComunaNombre ?? "");
                            doc.Direccion = direntrega;
                            doc.DirObservaciones = dir.Observaciones ?? "";
                            doc.HorarioAtencion = String.Format("{0} - {1}", dir.Ventana_Inicio, dir.Ventana_Termino);
                        }
                    }
                    else
                    {
                        doc.Direccion = "RETIRO EN PLANTA";
                        doc.DirObservaciones = "";
                        doc.HorarioAtencion = "";
                    }

                    doc.OCcliente = doc.Observaciones;

                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    foreach (var i in doc.Lineas)
                    {
                        json= repoStock.Get(i.ProdCode, i.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        i.StockActual = stock.Stock;
                        i.StockReceta = 0;
                        i.StockRecetaMP = 0;

                        if (i.TipoCode=="B")
                        {
                            json=repoRecea.List(i.ProdCode, "PRODUCCION");
                            var receta = JsonConvert.DeserializeObject<List<RecetaProductoList>>(json);
                            i.Receta = receta;
                            foreach(var r in receta)
                            {
                                i.StockReceta += r.Stock;
                            }

                            json = repoRecea.List(i.ProdCode, "TOLEDO");
                            receta = JsonConvert.DeserializeObject<List<RecetaProductoList>>(json);
                            i.Receta = receta;
                            foreach (var r in receta)
                            {
                                i.StockRecetaMP += r.Stock;
                            }
                        }
                    }
                    
                    var historial=this.Historial(docentry);
                    doc.Historial = JsonConvert.DeserializeObject<List<HistorialDoc>>(historial);
                }
                
                string JSONresult = JsonConvert.SerializeObject(doc);

                //string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OPED item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OPED.Find(item.DocEntry);
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
                var t = db.OPED.Find(docentry);
                if (t != null)
                {
                    db.OPED.Remove(t);
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
                var query = from e in db.OPED select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string ListVentas(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                //var query = from e in db.OPED where e.SocioCode==sociocode && e.EstadoOperativo!="NUL" && e.DocEstado=="A"  select e;
                var query = from e in db.spOPED_Ventas(sociocode,"A") select e; 
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string vendedorcode, string estadooperativo, string estado, int pendiente, string fechaini, string fechafin, string bodegacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOPED_Listar(vendedorcode, estadooperativo, estado, pendiente, fechaini,fechafin, bodegacode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Search(string palabras, string vendedorcode, string usuario)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spPedido_Busqueda(palabras, vendedorcode, usuario) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string estadooperativo)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OPED where e.EstadoOperativo == estadooperativo select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Historial(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOPED_Historial(docentry,10) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

    }
}
