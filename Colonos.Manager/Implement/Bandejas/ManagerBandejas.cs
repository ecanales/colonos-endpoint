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
    public class ManagerBandejas
    {
        Logger logger;
        public ManagerBandejas(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_OBDA repo = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                var obda = JsonConvert.DeserializeObject<OBDA>(item);
                obda.FechaIngreso = DateTime.Now;
                var json = repo.Add(obda);
                var bandejareturn= JsonConvert.DeserializeObject<Bandeja>(json);

                var items = JsonConvert.DeserializeObject<List<BDA1>>(item);

                foreach (var b in items)
                {
                    repoBandLinea.Add(b);
                    json = JsonConvert.SerializeObject(b);
                    var bb = JsonConvert.DeserializeObject<DocumentoLineaBandeja>(json);
                    bandejareturn.Items.Add(bb);
                }

                var list = JsonConvert.DeserializeObject<Socio>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Bandeja";
                msg.data = bandejareturn;
                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn Get(string bandejacode, int docentry)
        {
            try
            {
                Repo_OBDA repo = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                //Repo_OSCP repo = new Repo_OSCP(logger);
                Repo_SCP1 repodir = new Repo_SCP1(logger);
                Repo_SCP2 repocon = new Repo_SCP2(logger);

                var json = repo.Get(bandejacode, docentry);

                var bandeja = JsonConvert.DeserializeObject<Bandeja>(json);
                if (bandeja != null)
                {
                    json = repoBandLinea.List(bandejacode, docentry);
                    var items = JsonConvert.DeserializeObject<List<ItemBandeja>>(json);
                    bandeja.Items = items;
                }
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = bandeja==null ? "Bandeja no encontrada" : "Bandejas pedido";
                msg.data = bandeja;
                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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


        public MensajeReturn List(string bandejacode, bool estado, bool visible)
        {
            try
            {
                Repo_OBDA repo = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                var json = repo.List(bandejacode, estado, visible);
                var bandeja = JsonConvert.DeserializeObject<List<Bandeja>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Bandejas";
                msg.data = bandeja;
                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn List(string bandejacode, string sociocode)
        {
            try
            {
                Repo_OBDA repo = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                var json = repo.List(bandejacode, sociocode);
                var bandeja = JsonConvert.DeserializeObject<List<Bandeja>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Bandejas";
                msg.data = bandeja;
                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn List(string bandejacode, string sociocode, int top)
        {
            try
            {
                Repo_OBDA repo = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                var json = repo.List(bandejacode, sociocode, top);
                var bandeja = JsonConvert.DeserializeObject<List<Bandeja>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Bandejas";
                msg.data = bandeja;
                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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
            try
            {
                Repo_OBDA repo = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                var bandeja= JsonConvert.DeserializeObject<Bandeja>(item);

                //Repo_PED1 repolin = new Repo_PED1(logger);

                var lineasOriginal = JsonConvert.DeserializeObject<List<ItemBandeja>>(repoBandLinea.List(bandeja.BandejaCode, bandeja.DocEntry));
                var lineasUpdate = bandeja.Items;

                UpdateDetalle(lineasUpdate, lineasOriginal);
                var obda = JsonConvert.DeserializeObject<OBDA>(item);

                var json = repo.Modify(obda);

                if (Convert.ToBoolean(obda.Autorizado)) //Bandeja fue Autorizada
                {
                    ActivarsiguienteBandeja(obda.BandejaCode, obda.DocEntry);
                    if(VerificaAutorizacionCompleta(obda.DocEntry))
                    {
                        Repo_OPED repoped = new Repo_OPED(logger);
                        json = repoped.Get(obda.DocEntry);
                        var docgenerado=JsonConvert.DeserializeObject<Documento>(json);
                        GeneraPreparacionPedido(ref docgenerado);
                        //tu pedido está siendo preparado
                    }
                }
                else //Bandeja fue rechazada
                {
                    //anular pedido de venta
                    Repo_OPED repoped = new Repo_OPED(logger);
                    json = repoped.Get(bandeja.DocEntry);
                    var ped = JsonConvert.DeserializeObject<OPED>(json);
                    ped.EstadoOperativo = "NUL";
                    ped.BandejaCode = bandeja.BandejaCode;
                    ped.MotivoRechazo = bandeja.MotivoRech;
                    ped.DocEstado = "C";
                    repoped.Modify(ped);
                    //liberar cantidad asignada de cada item del pedido
                    Repo_PED1 repodet=new Repo_PED1(logger);
                    Repo_OITB repostock = new Repo_OITB();
                    json = repodet.List(ped.DocEntry);
                    var listDetalle = JsonConvert.DeserializeObject<List<DocumentoLinea>>(json);
                    foreach(var d in listDetalle)
                    {
                        json= repostock.Get(d.ProdCode, d.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        stock.Asignado -= d.CantidadPendiente;
                        repostock.Modify(stock);
                    }
                }
                var badejareturn = JsonConvert.DeserializeObject<Bandeja>(json);

                badejareturn.Items = JsonConvert.DeserializeObject<List<ItemBandeja>>(repoBandLinea.List(bandeja.BandejaCode, bandeja.DocEntry));
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Actualizar Bandeja";
                msg.data = badejareturn;
                return msg;
            }
            catch (Exception ex)
            {
                MensajeReturn msg = new MensajeReturn();
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


        private void UpdateDetalle(List<ItemBandeja> ItemsUpdate, List<ItemBandeja> ItemsCurr)
        {
            Repo_BDA1 repo = new Repo_BDA1();

            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (ItemBandeja ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<BDA1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<BDA1>(json);
                        repo.Delete(lin.DocEntry, lin.BandejaCode, lin.DocLinea);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<ItemBandeja> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        ItemBandeja cd = ItemsUpdate.Find(x => x.DocLinea == i.DocLinea);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<BDA1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<BDA1>(json);
                            repo.Delete(lin.DocEntry, lin.BandejaCode, lin.DocLinea);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<BDA1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }

        private void ActivarsiguienteBandeja(string bandejacode, int docentry)
        {
            //consulta jerarquia bandejas
            Repo_OBAN repo = new Repo_OBAN();
            var json = repo.Get(bandejacode);
            var sigbandeja = JsonConvert.DeserializeObject<OBAN>(json);
            if (sigbandeja != null)
            {
                Repo_OBDA repob = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                json=repob.Get(sigbandeja.BandejaCodeSiguiente, docentry);
                var bandeja= JsonConvert.DeserializeObject<Bandeja>(json);
                if (bandeja!=null)
                {
                    bandeja.FechaIngreso = DateTime.Now;
                    bandeja.UsuarioCodeIngreso = "";
                    bandeja.Visible = true;
                    bandeja.Items= JsonConvert.DeserializeObject<List<ItemBandeja>>(repoBandLinea.List(bandeja.BandejaCode, bandeja.DocEntry));
                    foreach (var b in bandeja.Items)
                    {
                        b.FechaIngreso = bandeja.FechaIngreso;
                        b.UsuarioCodeIngreso = bandeja.UsuarioCodeIngreso;
                    }
                    var lineasOriginal = JsonConvert.DeserializeObject<List<ItemBandeja>>(repoBandLinea.List(bandeja.BandejaCode, bandeja.DocEntry));
                    var lineasUpdate = bandeja.Items;

                    UpdateDetalle(lineasUpdate, lineasOriginal);
                    json = JsonConvert.SerializeObject(bandeja);
                    var obda = JsonConvert.DeserializeObject<OBDA>(json);
                    repob.Modify(obda);
                }
                
            }
        }

        public void GeneraPreparacionPedido(ref Documento docgenerado)
        {
            try
            {
                Repo_OPED repop = new Repo_OPED(logger);
                Repo_PED1 repod = new Repo_PED1(logger);
                //no quedan mas bandejas para autorizar, pedido cambia de estado a en preparación, se debe actualizar detalle del pedido con la %C y cantidad real y entregada a 0
                //tu pedido está siendo preparado
                var json = repop.Get(docgenerado.DocEntry);
                var ped = JsonConvert.DeserializeObject<OPED>(json);
                //var docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                ped.EstadoOperativo = "PREP";
                ped.FechaIngresoPrep = DateTime.Now;
                ped.Completado = 0;
                json = repod.List(docgenerado.DocEntry);
                var det = JsonConvert.DeserializeObject<List<PED1>>(json);
                foreach (var d in det)
                {
                    d.CantidadReal = 0;
                    d.CantidadEntregada = 0;
                    d.Completado = 0;
                    d.CantidadPendiente = d.CantidadSolicitada;
                    repod.Modify(d);
                }
                repop.Modify(ped);

                //******************************************************************************************
                //13-10-2024: Desde ahora la orden de producción se generará desde la badeja de TOLEDO -----
                //******************************************************************************************
                ////valida disponible actual del sku en TOLEDO ---------------
                ////sino queda disponible marcar saldo pendiente para
                ////generar orden de producción OPDC en bodega PRODUCCICION,
                ////se debe actualizar el disponible en PED1
                //Repo_OCFG_VTA repoConfig = new Repo_OCFG_VTA();
                //json = repoConfig.Get(1);
                //var config = JsonConvert.DeserializeObject<OCFG_VTA>(json);
                //Repo_OITB repoStock = new Repo_OITB();
                //Documento docProd = new Documento
                //{
                //    DocEstado = "A",
                //    DocTipo = 3010,
                //    DocFecha = docgenerado.DocFecha,
                //    SocioCode = docgenerado.SocioCode,
                //    RazonSocial = docgenerado.RazonSocial,
                //    VendedorCode = docgenerado.VendedorCode,
                //    UsuarioCode = docgenerado.UsuarioCode,
                //    Neto = docgenerado.Neto,
                //    Iva = docgenerado.Iva,
                //    Total = docgenerado.Total,
                //    FechaRegistro = Convert.ToDateTime(ped.FechaIngresoPrep),
                //    FechaIngresoPrep = Convert.ToDateTime(ped.FechaIngresoPrep),
                //    Version = docgenerado.Version,
                //    UsuarioNombre = docgenerado.UsuarioNombre,
                //    BaseEntry = docgenerado.DocEntry,
                //    BodegaCode = config.BodegaProduccion,
                //    BaseTipo = docgenerado.DocTipo,
                //    FechaEntrega=docgenerado.FechaEntrega,
                //    RetiraCliente=docgenerado.RetiraCliente,
                //    Lineas = new List<DocumentoLinea>()
                //};

                //var listProduccion = new List<DocumentoLinea>();
                //foreach (var i in docgenerado.Lineas)
                //{
                //    if (i.TipoCode == "B")
                //    {
                //        json = repoStock.Get(i.ProdCode, i.BodegaCode);
                //        var oitb = JsonConvert.DeserializeObject<OITB>(json);
                //        if (oitb != null)
                //        {
                //            decimal habian = Convert.ToDecimal(oitb.Stock) - (Convert.ToDecimal(oitb.Asignado) - Convert.ToDecimal(i.CantidadSolicitada));
                //            decimal enviaraproduccion = 0;
                //            if (true)
                //            {
                //                if (habian > 0)
                //                {
                //                    enviaraproduccion = habian >= Convert.ToDecimal(i.CantidadSolicitada) ? 0 : Convert.ToDecimal(i.CantidadSolicitada) - habian;
                //                }
                //                else if (habian <= 0)
                //                {
                //                    enviaraproduccion = Convert.ToDecimal(i.CantidadSolicitada);
                //                }
                //            }

                //            if (enviaraproduccion > 0)
                //            {
                //                docProd.Lineas.Add(new DocumentoLinea
                //                {
                //                    BaseEntry = i.DocEntry,
                //                    BaseLinea = i.DocLinea,
                //                    BaseTipo = i.DocTipo,

                //                    CantidadPendiente = Convert.ToDecimal(enviaraproduccion),
                //                    CantidadReal = 0,
                //                    CantidadSolicitada = Convert.ToDecimal(enviaraproduccion),
                //                    TotalSolicitado = Convert.ToDecimal(i.PrecioFinal) * enviaraproduccion,
                //                    TotalReal = 0,
                //                    BodegaCode = config.BodegaProduccion,

                //                    Descuento = Convert.ToDecimal(i.Descuento),
                //                    FechaConfirma = i.FechaConfirma,
                //                    LineaEstado = i.LineaEstado,
                //                    PrecioFinal = Convert.ToDecimal(i.PrecioFinal),
                //                    PrecioUnitario = Convert.ToDecimal(i.PrecioUnitario),
                //                    PrecioVolumen = Convert.ToDecimal(i.PrecioVolumen),
                //                    Disponible = Convert.ToDecimal(i.Disponible),

                //                    FactorPrecio = Convert.ToDecimal(i.FactorPrecio),
                //                    ProdCode = i.ProdCode,
                //                    ProdNombre = i.ProdNombre,
                //                    UsuarioCodeConfirma = i.UsuarioCodeConfirma,
                //                    FamiliaCode = i.FamiliaCode,
                //                    AnimalCode = i.AnimalCode,
                //                    Costo = i.Costo,
                //                    FormatoVtaCode = i.FormatoVtaCode,
                //                    Margen = i.Margen,
                //                    Medida = i.Medida,
                //                    TipoCode = i.TipoCode,
                //                    LineaItem = i.LineaItem,
                //                    Volumen = i.Volumen,

                //                    MargenRegla = i.MargenRegla,
                //                    AnimalNombre = i.AnimalNombre,
                //                    FamiliaNombre = i.FamiliaNombre,
                //                    FrmtoVentaNombre = i.FrmtoVentaNombre,
                //                    MarcaCode = i.MarcaCode,
                //                    MarcaNombre = i.MarcaNombre,
                //                    OrigenCode = i.OrigenCode,
                //                    OrigenNombre = i.OrigenNombre,
                //                    RefrigeraCode = i.RefrigeraCode,
                //                    RefrigeraNombre = i.RefrigeraNombre,
                //                    SolicitadoAnterior = 0,
                //                    CantidadEntregada = 0,
                //                    Completado = 0

                //                });
                //            }
                //        }
                //    }
                //}

                //if (docProd.Lineas.Any())
                //{

                //    Repo_OPDC repoProd = new Repo_OPDC(logger);
                //    json = repoProd.Add(docProd);
                //    var docprodgenerado = JsonConvert.DeserializeObject<Documento>(json);
                //    if (docprodgenerado != null && docprodgenerado.Lineas != null && docprodgenerado.Lineas.Any())
                //    {
                //        if (docgenerado.DocumentoReferencia == null)
                //            docgenerado.DocumentoReferencia = new List<Documento>();
                //        docgenerado.DocumentoReferencia.Add(docprodgenerado);
                //    }
                //}
                ////----------------------------------------------------------

            }
            catch (Exception ex)
            {
                logger.Error("Error al generar preparacion del pedido OPDC: {0}", ex.Message);
                logger.Error("{0}", ex.StackTrace);
            }

        }
        private bool VerificaAutorizacionCompleta(int docentry)
        {
            Repo_OBDA repob = new Repo_OBDA();
            

            var json= repob.List(docentry);
            var bandejas = JsonConvert.DeserializeObject<List<Bandeja>>(json);
            if (bandejas.Any())
            {
                if(!bandejas.FindAll(x => x.Estado == false).Any())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
