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
    public class Repo_OSMP
    {
        Logger logger;
        public Repo_OSMP(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OSMP where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var odoc = JsonConvert.DeserializeObject<OSMP>(JsonConvert.SerializeObject(item));

                    db.OSMP.Add(odoc);
                    db.SaveChanges();

                    int docentry = odoc.DocEntry;
                    item.DocEntry = docentry;

                    Repo_SMP1 repo = new Repo_SMP1();

                    foreach (var i in item.Lineas)
                    {
                        i.DocEntry = docentry;
                        i.DocTipo = Convert.ToInt32(item.DocTipo);

                        repo.Add(new SMP1
                        {
                            DocEntry = i.DocEntry,
                            DocLinea = i.DocLinea,
                            DocTipo = i.DocTipo,
                            BaseEntry = i.BaseEntry,
                            BaseLinea = i.BaseLinea,
                            BaseTipo = i.BaseTipo,
                            CantidadPendiente = Convert.ToDecimal(i.CantidadPendiente),
                            CantidadReal = Convert.ToDecimal(i.CantidadReal),
                            CantidadSolicitada = Convert.ToDecimal(i.CantidadSolicitada),
                            Descuento = Convert.ToDecimal(i.Descuento),
                            FechaConfirma = i.FechaConfirma,
                            LineaEstado = i.LineaEstado,
                            PrecioFinal = Convert.ToDecimal(i.PrecioFinal),
                            Disponible = Convert.ToDecimal(i.Disponible),
                            BodegaCode = i.BodegaCode,
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
                            AnimalNombre = i.AnimalNombre,
                            FamiliaNombre = i.FamiliaNombre,
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
                var query = from e in db.OSMP where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_SMP1 repo = new Repo_SMP1();
                    Repo_OITB repoStock = new Repo_OITB();
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                    foreach (var i in doc.Lineas)
                    {
                        var json = repoStock.Get(i.ProdCode, i.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        i.StockActual = stock.Stock;
                    }
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Modify(OSMP item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OSMP.Find(item.DocEntry);
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
                var t = db.OSMP.Find(docentry);
                if (t != null)
                {
                    db.OSMP.Remove(t);
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
                var query = from e in db.OSMP select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string estado)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOSMP_Listar(estado) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Search(string palabras, string vendedorcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spSolicitudMP_Busqueda(palabras, vendedorcode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
