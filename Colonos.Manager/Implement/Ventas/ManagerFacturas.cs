using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.Defontana;
using Colonos.Entidades;
using Colonos.Entidades.Defontana;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Manager
{
    public class ManagerFacturas
    {
        Logger logger;
        cnnDF cnndf;
        public ManagerFacturas(Logger _logger, cnnDF _cnnDF)
        {
            logger = _logger;
            cnndf = _cnnDF;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {


                Repo_OFAC repo = new Repo_OFAC(logger);
                Repo_OCFG_VTA repoConfig = new Repo_OCFG_VTA();
                Repo_OSCP repocli = new Repo_OSCP(logger);
                Repo_SCP1 repodir=new Repo_SCP1(logger);
                Repo_SCP12 repocond = new Repo_SCP12();


                var json= repocli.Get(doc.SocioCode);
                var cli = JsonConvert.DeserializeObject<OSCP>(json);
                json = repodir.List(cli.SocioCode);
                var dirlist = JsonConvert.DeserializeObject<List<SCP1>>(json);
                var dirfac = dirlist.Find(x => x.DireccionTipo == "F");
                json = repocond.Get(doc.CondicionDF);
                var cond = JsonConvert.DeserializeObject<SCP12>(json);

                var direntrega = dirlist.Find(x => x.DireccionCode == doc.DireccionCode);
                if (dirfac != null)
                {
                    Repo_ITM5 repoAnimal = new Repo_ITM5();
                    json = repoAnimal.List();
                    var animal = JsonConvert.DeserializeObject<List<ITM5>>(json);

                    //primero grabar en Defontana ------
                    AgenteDefontana df = new AgenteDefontana(logger, cnndf);
                    DocumentoDF fac = new DocumentoDF { details = new List<Details>() };
                    fac.emissionDate = new EmissionDate { day = doc.DocFecha.Day, month = doc.DocFecha.Month, year = doc.DocFecha.Year };
                    var fechavencimiento = doc.DocFecha.AddDays(Convert.ToInt64(cond.daysBetweenPayments));
                    fac.firstFeePaid = new EmissionDate { day = fechavencimiento.Day, month = fechavencimiento.Month, year = fechavencimiento.Year };
                    fac.clientFile = cli.clientFileDF;
                    fac.contactIndex = String.Format("{0} {1}", dirfac.Calle, dirfac.Numero);
                    fac.documentType = cnndf.docfactura;

                    fac.rutMandante = "";
                    fac.paymentCondition = cond.code;
                    fac.sellerFileId = doc.VendedorCode;
                    fac.clientAnalysis = new Analysis { accountNumber = cnndf.accclientesnacionales, businessCenter = "", classifier01 = "", classifier02 = "" };
                    fac.billingCoin = "PESO";
                    fac.billingRate = 1;
                    fac.shopId = "GVL000000000000";
                    fac.priceList = "1";
                    fac.giro = cli.Giro;
                    fac.district = "";
                    fac.city = dirfac.CiudadNombre;
                    fac.district = dirfac.ComunaNombre;
                    fac.contact = -1;
                    
                    if(doc.Observaciones !=null && doc.Observaciones!="" && doc.Observaciones.Trim().Length>0)
                    {
                        fac.attachedDocuments = new List<attachedDocument> { 
                        new attachedDocument
                        {
                            date=new EmissionDate
                            {
                                day=doc.DocFecha.Day,
                                month=doc.DocFecha.Month,
                                year=doc.DocFecha.Year
                            },
                            documentTypeId="801",
                            folio=doc.Observaciones,
                            reason="Orden de Compra"
                        }
                        };
                    }
                    fac.storage = new Storage { code = "", motive = "", storageAnalysis = new Analysis { accountNumber = "", businessCenter = "", classifier01 = "", classifier02 = "" } };
                    fac.saleTaxes.Add(new SaleTaxes { code = "IVA", value = cnndf.valoriva, taxeAnalysis = new Analysis { accountNumber = cnndf.acciva, businessCenter = "", classifier01 = "", classifier02 = "" } });
                    fac.ventaRecDesGlobal = new List<string>();
                    string fechaentrega = "";
                    if (doc.FechaEntrega != null)
                        fechaentrega = String.Format("Entrega: {0:dd/MM/yyyy}", doc.FechaEntrega);

                    if (direntrega != null)
                    {
                        

                        fac.gloss = String.Format("OV:{6} {0} {1} DE {2} A {3} HRS {4} {5}", direntrega.Calle + " " + direntrega.Numero, direntrega.ComunaNombre, direntrega.Ventana_Inicio == "" ? "9:00" : direntrega.Ventana_Inicio, direntrega.Ventana_Termino == "" ? "20:00" : direntrega.Ventana_Termino, fechaentrega, direntrega.Observaciones, doc.BaseEntry);
                    }
                    else
                    {
                        fac.gloss = String.Format("OV:{0} {1} {2}", doc.BaseEntry,"RETIRA EN PLANTA", fechaentrega);
                    }
                    fac.customFields = new List<string>();
                    fac.isTransferDocument = true; //false; //true = documento preliminar (no se envia al sii), false = documento final (se envia al sii)
                    
                    foreach(var d in doc.Lineas)
                    {
                        var animalselect = animal.Find(x => x.AnimalCode == d.AnimalCode);
                        if(animalselect == null)
                        {
                            animalselect = new ITM5 { AccDF = cnndf.accventaingresos };
                        }
                        if (animalselect.AccDF == null)
                        {
                            animalselect.AccDF = cnndf.accventaingresos;
                        }
                        
                        fac.details.Add(new Details
                        {
                            
                            type = "A",
                            isExempt = false,
                            code = d.ProdCode,
                            count = Convert.ToDecimal(d.CantidadSolicitada),
                            productName = d.ProdNombre,
                            productNameBarCode = d.ProdNombre,
                            comment="",
                            price = Convert.ToDecimal(d.PrecioFinal),
                            discount = new Discount { type = 0, value = 0 },
                            unit = d.Medida,
                            analysis = new Analysis { accountNumber = animalselect.AccDF, businessCenter = "GVL006000000000", classifier01 = "", classifier02 = "" },
                            useBatch = false,
                            batchInfo = new List<string>()
                        }); ;
                    }

                    var token = df.RenovarToken(cnndf.metodoauth);
                    var result = "";
                    bool succes = false;
                    if (token!="")
                    {
                        
                        json=JsonConvert.SerializeObject(fac,Formatting.Indented);
                        result=df.ExecutePost(cnndf.metodofacturas, token, json, ref succes);
                        
                    }
                    //----------------------------------
                    var docdf = JsonConvert.DeserializeObject<SaveSaleReturn>(result);
                    if (succes)
                    {
                        if (docdf.success)
                        {
                            doc.FolioDF = docdf.firstFolio;
                            doc.DireccionCodeFact = dirfac.DireccionCode;
                            doc.clientFileDF = fac.clientFile;

                            json = repo.Add(doc);
                            //actualizar estado pedido y sus lineas que fueron facturadas

                            var docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                            msg = new MensajeReturn();
                            msg.statuscode = HttpStatusCode.OK;
                            msg.count = 1;
                            msg.error = false;
                            msg.msg = String.Format("Factura Generada: {0}",docgenerado.FolioDF);
                            msg.data = docgenerado;
                            return msg;
                        }
                        else
                        {
                            msg = new MensajeReturn();
                            msg.statuscode = HttpStatusCode.OK;
                            msg.count = 0;
                            msg.error = true;
                            msg.msg = String.Format("Factura no pudo ser generada en DF: {0}", docdf.message);
                            msg.data = null;
                            return msg;
                        }
                    }
                    else
                    {
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.OK;
                        msg.count = 0;
                        msg.error = true;
                        if (docdf != null)
                        {
                            msg.msg = String.Format("Factura no pudo ser generada en DF: {0}", docdf.message);
                        }
                        else
                        {
                            msg.msg = String.Format("Factura no pudo ser generada en DF ");
                        }
                        msg.data = null;
                        return msg;
                    }
                }
                else
                {
                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.OK;
                    msg.count = 0;
                    msg.error = true;
                    msg.msg = "Cliente no tiene una dirección de Facturación";
                    msg.data = null;
                    return msg;
                }

            }
            catch (Exception ex)
            {
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.InternalServerError;
                msg.error = true;
                msg.msg = ex.Message;
                msg.data = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    msg.data += JsonConvert.SerializeObject(ex);
                }

                return msg;
            }
        }

        public MensajeReturn Modify(string item)
        {

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);

            Repo_OFAC repo = new Repo_OFAC(logger);
            Repo_FAC1 repolin = new Repo_FAC1(logger);
            var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            var linupdate = doc.Lineas;

            UpdateDetalle(linupdate, linoriginal);
            var oped = JsonConvert.DeserializeObject<OFAC>(item);

            var json = repo.Modify(oped);
            var ped = JsonConvert.DeserializeObject<Documento>(json);
            ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.msg = ped == null ? "Factura no existe" : "Actualizar Factura";
            msg.data = ped;

            return msg;
        }

        public MensajeReturn Search(string palabras, string vendedocode)
        {
            Repo_OFAC repo = new Repo_OFAC(logger);

            var json = repo.Search(palabras, vendedocode);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Facturas";
            msg.data = list;
            return msg;
        }

        public MensajeReturn List()
        {
            Repo_OFAC repo = new Repo_OFAC(logger);
            var json = repo.List();
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Facturas";
            msg.data = list;
            return msg;
        }

        public MensajeReturn List(string estado, string fechaini, string fechafin)
        {
            Repo_OFAC repo = new Repo_OFAC(logger);
            var json = repo.List(estado, fechaini, fechafin);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Facturas";
            msg.data = list;
            return msg;
        }

        public MensajeReturn Get(int docentry)
        {
            Repo_OFAC repo = new Repo_OFAC(logger);
            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Pedido no existe" : "Pedido";
            msg.data = doc;

            return msg;
        }

        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_FAC1 repo = new Repo_FAC1(logger);
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<FAC1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<FAC1>(json);
                        repo.Delete(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<DocumentoLinea> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        DocumentoLinea cd = ItemsUpdate.Find(x => x.DocLinea == i.DocLinea);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<FAC1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<FAC1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<FAC1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
    }

}
