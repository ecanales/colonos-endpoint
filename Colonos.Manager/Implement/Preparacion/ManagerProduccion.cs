using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.Entidades;
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
    public class ManagerProduccion
    {
        Logger logger;

        public ManagerProduccion(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {
                //--------------------------------------
                //1- validar que no tenga una orden de producción para el pedido de ventas OPED.Doctypo = 10
                Repo_OPDC repoProd = new Repo_OPDC(logger);
                var json = repoProd.GetBase(Convert.ToInt32(doc.BaseEntry), Convert.ToInt32(doc.BaseTipo));
                var docbase = JsonConvert.DeserializeObject<Documento>(json);
                var tieneOPDC = false;
                if (docbase != null && docbase.DocEntry != null && docbase.DocEstado=="A")
                {
                    //tiene una orden de produccion generada para el pedido de venta, no generar nuevo OPDC
                    throw new Exception(String.Format("Pedido: {0} ya tiene una Solicitud de Producción generada: {1}", doc.BaseEntry, docbase.DocEntry));
                }
                //--------------------------------------
                Repo_OPDC repo = new Repo_OPDC(logger);
                doc.FechaRegistro = DateTime.Now;
                json = repo.Add(doc);
                

                var docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                //20-10-2024: marcar items del pedido de venta que fueron enviados a producción
                if(docgenerado.BaseTipo==10) // es generado desde un pedido de venta
                {
                    //recorrer el detalle de la orden de producción e ir marcando el item del pedido de venta
                    Repo_PED1 repoped = new Repo_PED1(logger);
                    foreach(var i in docgenerado.Lineas)
                    {
                        json = repoped.Get(Convert.ToInt32(i.BaseLinea));
                        var ped1 = JsonConvert.DeserializeObject<PED1>(json);
                        if (ped1!=null)
                        {
                            ped1.EnProduccion = "S";
                            repoped.Modify(ped1);
                        }
                    }
                }
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Solicitud de Producción Generado";
                msg.data = docgenerado;
                return msg;
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

        public MensajeReturn Get(int docentry)
        {
            Repo_OPDC repo = new Repo_OPDC(logger);
            
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

        public MensajeReturn Modify(string item)
        {

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);

            Repo_OPDC repo = new Repo_OPDC(logger);
            Repo_PDC1 repolin = new Repo_PDC1();
            var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            var linupdate = doc.Lineas;

            UpdateDetalle(linupdate, linoriginal);
            var oped = JsonConvert.DeserializeObject<OPDC>(item);

            var json = repo.Modify(oped);
            var ped = JsonConvert.DeserializeObject<Documento>(json);
            ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.msg = ped == null ? "Solicitud no existe" : "Actualizar Solicitud";
            msg.data = ped;

            return msg;
        }

        public MensajeReturn List(string bodegacode, string estado)
        {
            Repo_OPDC repo = new Repo_OPDC(logger);
            var json = repo.List(estado);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Producción";
            msg.data = list;
            return msg;
        }

        public MensajeReturn ModifyEstadoItem(string item)
        {

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);
            //Repo_OITB repostock = new Repo_OITB();
            Repo_OPDC repo = new Repo_OPDC(logger);
            Repo_PDC1 repolin = new Repo_PDC1();

            var json = repo.Get(doc.DocEntry);
            var pedActual = JsonConvert.DeserializeObject<Documento>(json);


            MensajeReturn msg = new MensajeReturn();
            if (pedActual.DocEstado=="A")
            {
                var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var linupdate = doc.Lineas;


                UpdateDetalle(linupdate, linoriginal);
                var opdc = JsonConvert.DeserializeObject<OPDC>(item);

                linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var cerrarpedido = true;
                foreach (var i in linoriginal)
                {
                    if (i.LineaEstado == "A")
                    {
                        cerrarpedido = false;
                    }
                }
                if (cerrarpedido)
                {
                    opdc.DocEstado = "C";

                    //*************************************************************
                    //08-11-2024: liberar item del pedido relacionado
                    Documento oped = null;
                    var repoPed = new Repo_OPED(logger);
                    var repoPedDet = new Repo_PED1(logger);
                    if (opdc.BaseTipo == 10)
                    {

                        json = repoPed.Get(Convert.ToInt32(opdc.BaseEntry));
                        oped = JsonConvert.DeserializeObject<Documento>(json);
                    }
                    if (opdc.BaseTipo == 10 && oped != null && oped.DocEstado != "C" && oped.Lineas != null && oped.Lineas.Any())
                    {
                        foreach (var pdc1 in linoriginal)
                        {
                            var ped1 = oped.Lineas.Find(x => x.DocLinea == pdc1.BaseLinea && x.DocEntry == pdc1.BaseEntry && x.DocTipo == pdc1.BaseTipo);
                            if (ped1 != null)
                            {
                                ped1.EnProduccion = null;
                                json = JsonConvert.SerializeObject(ped1);
                                var det = JsonConvert.DeserializeObject<PED1>(json);
                                repoPedDet.Modify(det);
                            }
                        }
                    }

                }
                json = repo.Modify(opdc);

                var ped = JsonConvert.DeserializeObject<Documento>(json);

                ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Pedido no existe" : "Actualizar Pedido";
                msg.data = ped;

                return msg;
            }
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = true;
            msg.msg = String.Format("Pedido no puede ser modificado. Estado Actual: {0}", pedActual.DocEstado);
            msg.data = "";

            return msg;
        }

        public MensajeReturn Search(string palabras, string vendedocode)
        {
            Repo_OPDC repo = new Repo_OPDC(logger);

            var json = repo.Search(palabras, vendedocode);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Pedidos";
            msg.data = list;
            return msg;
        }


        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_PDC1 repo = new Repo_PDC1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<PDC1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PDC1>(json);
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
                            var lin = JsonConvert.DeserializeObject<PDC1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<PDC1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PDC1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
    }
}
