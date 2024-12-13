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
    public class Repo_OLOG
    {
        Logger logger;
        public Repo_OLOG(Logger _logger)
        {
            logger = _logger;
        }

        public string Add(Documento item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OLOG where e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    var olog = JsonConvert.DeserializeObject<OLOG>(JsonConvert.SerializeObject(item));

                    db.OLOG.Add(olog);
                    db.SaveChanges();

                    
                    int docentry = olog.DocEntry;
                    item.DocEntry = docentry;
                    Repo_LOG1 repo = new Repo_LOG1();

                    foreach (var i in item.Lineas)
                    {
                        i.DocEntry = docentry;
                        i.DocTipo = item.DocTipo;

                        repo.Add(new LOG1
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
                            Completado = i.Completado,
                            
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
                var query = from e in db.OLOG where e.DocEntry == docentry select e;
                var item = query.FirstOrDefault();

                var doc = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(item));
                if (doc != null)
                {
                    Repo_LOG1 repo = new Repo_LOG1();
                    var lineas = repo.List(item.DocEntry);
                    doc.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(lineas);
                }

                string JSONresult = JsonConvert.SerializeObject(doc);

                return JSONresult;
            }
        }

        public string Modify(OLOG item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OLOG.Find(item.DocEntry);
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
                var t = db.OLOG.Find(docentry);
                if (t != null)
                {
                    db.OLOG.Remove(t);
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
                var query = from e in db.OLOG select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string estado, string fechaini, string fechafin)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOLOG_Listar(estado, fechaini, fechafin,"") select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
        public string List(string scenario_token)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OLOG where e.scenario_token == scenario_token select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
        public string Search(string palabras, string vendedorcode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spLogistica_Busqueda(palabras, vendedorcode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

    }
}
