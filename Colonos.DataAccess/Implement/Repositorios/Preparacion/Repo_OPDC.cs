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
    public class Repo_OPDC
    {
        Logger logger;
        public Repo_OPDC(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OPDC where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var opkg = JsonConvert.DeserializeObject<OPDC>(JsonConvert.SerializeObject(item));

                    db.OPDC.Add(opkg);
                    db.SaveChanges();

                    int docentry = opkg.DocEntry;
                    item.DocEntry = docentry;

                    Repo_PDC1 repo = new Repo_PDC1();

                    foreach (var i in item.Lineas)
                    {
                        i.DocEntry = docentry;
                        i.DocTipo =Convert.ToInt32(item.DocTipo);

                        repo.Add(new PDC1
                        {
                            DocEntry = i.DocEntry,
                            DocLinea = i.DocLinea,
                            DocTipo = i.DocTipo,
                            BaseEntry = i.BaseEntry,
                            BaseLinea = i.BaseLinea,
                            BaseTipo=i.BaseTipo,

                            
                            CantidadPendiente = Convert.ToDecimal(i.CantidadPendiente),
                            CantidadReal = Convert.ToDecimal(i.CantidadReal),
                            CantidadSolicitada = Convert.ToDecimal(i.CantidadSolicitada),
                            Descuento = Convert.ToDecimal(i.Descuento),
                            FechaConfirma = i.FechaConfirma,
                            LineaEstado = i.LineaEstado,
                            PrecioFinal = Convert.ToDecimal(i.PrecioFinal),
                            PrecioUnitario = Convert.ToDecimal(i.PrecioUnitario),
                            PrecioVolumen = Convert.ToDecimal(i.PrecioVolumen),
                            Disponible = Convert.ToDecimal(i.Disponible),
                            BodegaCode = i.BodegaCode,
                            FactorPrecio = Convert.ToDecimal(i.FactorPrecio),
                            ProdCode = i.ProdCode,
                            ProdNombre = i.ProdNombre,
                            UsuarioCodeConfirma = i.UsuarioCodeConfirma,
                            FamiliaCode = i.FamiliaCode,
                            AnimalCode = i.AnimalCode,
                            Costo = i.Costo,
                            FormatoVtaCode = i.FormatoVtaCode,
                            Margen = i.Margen,
                            Medida = i.Medida,
                            TipoCode = i.TipoCode,
                            LineaItem = i.LineaItem,
                            Volumen = i.Volumen,
                            TotalSolicitado = i.TotalSolicitado,
                            TotalReal = i.TotalReal,
                            MargenRegla = i.MargenRegla,
                            AnimalNombre = i.AnimalNombre,
                            FamiliaNombre = i.FamiliaNombre,
                            FrmtoVentaNombre = i.FrmtoVentaNombre,
                            MarcaCode = i.MarcaCode,
                            MarcaNombre = i.MarcaNombre,
                            OrigenCode = i.OrigenCode,
                            OrigenNombre = i.OrigenNombre,
                            RefrigeraCode = i.RefrigeraCode,
                            RefrigeraNombre = i.RefrigeraNombre,
                            SolicitadoAnterior = 0,
                            CantidadEntregada = i.CantidadEntregada,
                            Completado = i.Completado
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
                var query = from e in db.OPDC where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_PDC1 repo = new Repo_PDC1();
                    Repo_OITB repoStock = new Repo_OITB();
                    Repo_ITR1 repoReceta = new Repo_ITR1();

                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    foreach (var i in doc.Lineas)
                    {
                        var json = repoStock.Get(i.ProdCode, i.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        i.StockActual = stock.Stock;
                        decimal stockreceta = 0;
                        if (true)
                        {
                            //sumar la cantidad total se stock de la receta
                            json = repoReceta.List(i.ProdCode, i.BodegaCode);
                            var stockrecetaActual = JsonConvert.DeserializeObject<List<OITB>>(json);
                            if (stockrecetaActual != null && stockrecetaActual.Any())
                            {
                                foreach (var s in stockrecetaActual)
                                {
                                    stockreceta += Convert.ToDecimal(s.Stock);
                                }
                                i.StockActual = stockreceta;
                            }
                        }
                    }
                    
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Modify(OPDC item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OPDC.Find(item.DocEntry);
                if (t != null)
                {
                    //Si se esta cerrando la orden de prod cerrar detalle tambien
                    if(item.DocEstado=="C")
                    {
                        Repo_PDC1 repo = new Repo_PDC1();
                        var json = repo.List(item.DocEntry);
                        var list = JsonConvert.DeserializeObject<List<PDC1>>(json);
                        foreach (PDC1 d in list)
                        {
                            d.LineaEstado = "C";
                            repo.Modify(d);
                        }
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

        public bool Delete(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OPDC.Find(docentry);
                if (t != null)
                {
                    db.OPDC.Remove(t);
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
                var query = from e in db.OPDC select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string estado)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOPDC_Listar(estado) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Search(string palabras, string vendedorcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spProduccion_Busqueda(palabras, vendedorcode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }


        public string GetBase(int baseentry, int basetipo)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OPDC where e.BaseEntry == baseentry && e.BaseTipo==basetipo select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_PKG3 repo = new Repo_PKG3();
                    Repo_OITB repoStock = new Repo_OITB();

                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }
    }
}
