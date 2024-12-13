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
    public class Repo_OPKG
    {
        Logger logger;
        
        public Repo_OPKG(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OPKG where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    //04-09-2024: solo generar el picking si hay stock de almenos un item ----
                    var hayStock = false;
                    var json = "";
                    OITB oitb;
                    foreach (var i in item.Lineas)
                    {
                        var prodcode = i.ProdCode;
                        var bodegacode = i.BodegaCode;

                        Repo_OITB repostock = new Repo_OITB();
                        if(i.TieneReceta!=null && i.TieneReceta=="S" && i.TipoCode=="B")
                        {
                            //obtener la receta
                            Repo_ITR1 reporeceta = new Repo_ITR1();
                            json = reporeceta.List(prodcode,bodegacode);
                            var listreceta = JsonConvert.DeserializeObject<List<ITR1>>(json);
                            if(listreceta!=null && listreceta.Any())
                            {
                                foreach(var r in listreceta)
                                {
                                    prodcode = r.ProdCodeRef;

                                    json = repostock.Get(prodcode, bodegacode);
                                    oitb = JsonConvert.DeserializeObject<OITB>(json);
                                    if (oitb != null)
                                    {
                                        if (oitb.Stock > 0)
                                            hayStock = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                json = repostock.Get(prodcode, bodegacode);
                                oitb = JsonConvert.DeserializeObject<OITB>(json);
                                if (oitb != null)
                                {
                                    if (oitb.Stock > 0)
                                        hayStock = true;
                                    break;
                                }
                            }
                        }

                        //json = repostock.Get(prodcode, bodegacode);

                        //oitb = JsonConvert.DeserializeObject<OITB>(json);
                        //if (oitb != null)
                        //{
                        //    if (oitb.Stock > 0)
                        //        hayStock = true;
                        //    break;
                        //}
                    }
                    //05-09-2024: se reversa regla
                    hayStock = true;
                    if (hayStock)
                    {
                        //------------------------------------------------------------------------
                        var opkg = JsonConvert.DeserializeObject<OPKG>(JsonConvert.SerializeObject(item));

                        db.OPKG.Add(opkg);
                        db.SaveChanges();

                        int docentry = opkg.DocEntry;
                        item.DocEntry = docentry;


                        Repo_PKG1 repo = new Repo_PKG1();

                        foreach (var i in item.Lineas)
                        {
                            i.DocEntry = docentry;
                            i.DocTipo = item.DocTipo;

                            repo.Add(new PKG1
                            {
                                DocEntry = i.DocEntry,
                                DocLinea = i.DocLinea,
                                DocTipo = i.DocTipo,
                                BaseEntry = i.BaseEntry,
                                BaseLinea = i.BaseLinea,
                                BaseTipo = i.BaseTipo,

                                CantidadReal = 0,
                                CantidadSolicitada = Convert.ToDecimal(i.CantidadSolicitada),
                                BodegaCode = i.BodegaCode,
                                ProdCode = i.ProdCode,
                                ProdNombre = i.ProdNombre,
                                MarcaNombre = i.MarcaNombre,
                                OrigenNombre = i.OrigenNombre,
                                RefrigeraNombre = i.RefrigeraNombre,
                                TieneReceta = i.TieneReceta,
                                FrmtoVentaNombre = i.FrmtoVentaNombre,
                                Cajas = i.Cajas,
                                KilosPorCaja = i.KilosPorCaja,
                                Diferencia = i.Diferencia,
                                Tolerancia = i.Tolerancia,
                                Completado = i.Completado,
                                TipoCode = i.TipoCode,
                            });
                        }
                        return Get(docentry);
                    }
                    else
                    {
                        throw new ArgumentException("productos no tienen stock para generar el Picking");
                    }
                }
            }
            return JSONresult;
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OPKG where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_PKG1 repo = new Repo_PKG1();
                    Repo_OITB repoStock = new Repo_OITB();
                    Repo_OITM repoProd = new Repo_OITM(logger);

                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    foreach (var i in doc.Lineas)
                    {
                        var json = repoStock.Get(i.ProdCode, i.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        json = repoProd.Get(i.ProdCode);
                        var prod = JsonConvert.DeserializeObject<OITM>(json);
                        i.StockActual = stock.Stock;
                        if(doc.DocEstado=="A")
                            i.TieneReceta = prod.TieneReceta;
                    }
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
                    Repo_OITB repoStock = new Repo_OITB();

                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    foreach (var l in doc.Lineas)
                    {
                        var json= repoStock.Get(l.ProdCode, l.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        
                        if (stock!=null)
                        {
                            l.StockActual = stock.Stock;
                        }
                    }

                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string GetBaseLinea(int baseentry, int baselinea)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG2 where e.BaseEntry == baseentry && e.BaseLinea==baselinea select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_PKG3 repo = new Repo_PKG3();
                    Repo_OITB repoStock = new Repo_OITB();

                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    foreach (var l in doc.Lineas)
                    {
                        var json = repoStock.Get(l.ProdCode, l.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);

                        if (stock != null)
                        {
                            l.StockActual = stock.Stock;
                        }
                    }

                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Modify(OPKG item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OPKG.Find(item.DocEntry);
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
                var t = db.OPKG.Find(docentry);
                if (t != null)
                {
                    db.OPKG.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public string List(int baseentry, int basetipo)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OPKG where e.BaseEntry== baseentry && e.BaseTipo== basetipo select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string bodegacode, string estado)
        {
            using (var db = new cnnDatos())
            {
                //var query = from e in db.spPedido_BandejaBodega(bodegacode) select e;
                var query = from e in db.spOPKG_Listar(bodegacode,estado) select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string ListPicking(string bodegacode, string estado)
        {
            using (var db = new cnnDatos())
            {
                //var query = from e in db.spPicking_Listar(bodegacode, estado) select e;
                var query = from e in db.spOPKG_Listar(bodegacode, estado) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Picking(int docentry, string origen)
        {
            using (var db = new cnnDatos())
            {
                string JSONresult = "";
                if (origen == "produccion")
                {
                    var query = from e in db.spPicking_Produccion(docentry) select e;
                    var item = query.FirstOrDefault();

                    var doc = JsonConvert.DeserializeObject<Picking>(JsonConvert.SerializeObject(item));
                    if (doc != null)
                    {
                        Repo_PKG1 repo = new Repo_PKG1();

                        var lineas = repo.PickingDetalle(item.DocEntry, origen);
                        doc.Lineas = JsonConvert.DeserializeObject<List<Picking_Lineas>>(lineas);
                    }
                    JSONresult = JsonConvert.SerializeObject(doc);
                }
                if (origen == "toledo")
                {
                    var query = from e in db.spPicking_Toledo(docentry) select e;
                    var item = query.FirstOrDefault();

                    var doc = JsonConvert.DeserializeObject<Picking>(JsonConvert.SerializeObject(item));
                    if (doc != null)
                    {
                        Repo_PKG1 repo = new Repo_PKG1();

                        var lineas = repo.PickingDetalle(item.DocEntry, origen);
                        doc.Lineas = JsonConvert.DeserializeObject<List<Picking_Lineas>>(lineas);
                    }
                    JSONresult = JsonConvert.SerializeObject(doc);
                }
                return JSONresult;
            }
        }

        

        public string Search(string palabras, string bodegacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spPicking_Busqueda(palabras, bodegacode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
