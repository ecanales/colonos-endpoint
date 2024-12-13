using Colonos.DataAccess.Repositorios;
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
    public class Repo_OFAC
    {
        Logger logger;
        public Repo_OFAC(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OFAC where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    Repo_OITM repoprod = new Repo_OITM(logger);
                    var ofac = JsonConvert.DeserializeObject<OFAC>(JsonConvert.SerializeObject(item));

                    db.OFAC.Add(ofac);
                    db.SaveChanges();

                    int docentry = ofac.DocEntry;
                    item.DocEntry = docentry;

                    Repo_FAC1 repo = new Repo_FAC1(logger);
                    var json = "";
                    foreach (var i in item.Lineas)
                    {
                        json = repoprod.Get(i.ProdCode);
                        var prod = JsonConvert.DeserializeObject<OITM>(json);

                        i.DocEntry = docentry;
                        repo.Add(new FAC1
                        {
                            DocEntry = i.DocEntry,
                            DocLinea = i.DocLinea,
                            BaseEntry = i.BaseEntry,
                            BaseLinea = i.BaseLinea,
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
                            FamiliaCode = prod.FamiliaCode,
                            AnimalCode = prod.AnimalCode,
                            Costo = prod.Costo,
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

                    //-- cerrar oped si todos los item estan cerrados
                    Repo_OPED repoped = new Repo_OPED(logger);
                    json= repoped.Get(Convert.ToInt32(ofac.BaseEntry));
                    var doc= JsonConvert.DeserializeObject<Documento>(json);
                    var oped = JsonConvert.DeserializeObject<OPED>(json);
                    if (oped.Facturado == null)
                        oped.Facturado = 0;
                    oped.Facturado += ofac.Total;

                    var lin = doc.Lineas.FindAll(x => x.LineaEstado == "A").Count;
                    if(lin == 0){
                        
                        oped.DocEstado = "C";
                        
                    }
                    repoped.Modify(oped);
                    return Get(docentry);
                    //JSONresult = JsonConvert.SerializeObject(item);
                    //return JSONresult;
                }
            }

            //JSONresult = JsonConvert.SerializeObject(item);
            ////JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OFAC where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_FAC1 repo = new Repo_FAC1(logger);
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                //string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OFAC item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OFAC.Find(item.DocEntry);
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
                var t = db.OFAC.Find(docentry);
                if (t != null)
                {
                    db.OFAC.Remove(t);
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
                var query = from e in db.OFAC select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string estado,string fechaini, string fechafin)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOFAC_Listar(estado,fechaini, fechafin) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string Search(string palabras, string vendedorcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spFactura_Busqueda(palabras, vendedorcode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
