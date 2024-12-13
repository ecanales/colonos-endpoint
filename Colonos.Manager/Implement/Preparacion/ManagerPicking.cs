using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
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
    public class ManagerPicking
    {
        Logger logger;
        cnnDF cnndf;
        public ManagerPicking(Logger _logger, cnnDF _cnndf)
        {
            logger = _logger;
            cnndf = _cnndf;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {
                Repo_OPKG repo = new Repo_OPKG(logger);
                var json = repo.List(Convert.ToInt32(doc.BaseEntry), Convert.ToInt32(doc.BaseTipo));
                var list = JsonConvert.DeserializeObject<List<OPKG>>(json);
                if (list.Any())
                {
                    if (list.FindAll(x => x.DocEstado == "A" && x.BodegaCode == doc.BodegaCode).Any())
                    {
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.Ambiguous;
                        msg.error = true;
                        msg.msg = "Ya Existe un picking activo";
                        msg.data = null;

                        return msg;
                    }
                }

                doc.FechaRegistro = DateTime.Now;
                json = repo.Add(doc);
                var docgenerado = JsonConvert.DeserializeObject<Documento>(json);


                //15-10-2024: NO APLICA - ORDEN PRODUCCION SE GENERA EN OPCION INDEPENDIENTE EN BANDEJA TOLEDO ----------------------
                if (doc.BaseTipo == 10 && 1==2) 
                {

                    
                    //13-10-2024: Generar orden de producción si tiene productos B sin stock en Toledo ----------------------
                    //valida disponible actual del sku en TOLEDO ---------------
                    //sino queda disponible marcar saldo pendiente para
                    //generar orden de producción OPDC en bodega PRODUCCICION,
                    //se debe actualizar el disponible en PED1

                    //1- validar que no tenga una orden de producción para el pedido de ventas OPED.Doctypo = 10
                    Repo_OPDC repoProd = new Repo_OPDC(logger);
                    json = repoProd.GetBase(Convert.ToInt32(doc.BaseEntry), Convert.ToInt32(doc.BaseTipo));
                    var docbase = JsonConvert.DeserializeObject<Documento>(json);
                    var tieneOPDC = false;
                    if (docbase != null && docbase.DocEntry != null)
                    {
                        //tiene una orden de produccion generada para el pedido de venta, no generar nuevo OPDC
                        tieneOPDC = true;
                    }

                    if (!tieneOPDC)
                    {
                        //2- validar si picking tiene producto B, construir detalle OPDC ---
                        Repo_OCFG_VTA repoConfig = new Repo_OCFG_VTA();
                        json = repoConfig.Get(1);
                        var config = JsonConvert.DeserializeObject<OCFG_VTA>(json);
                        Repo_OITB repoStock = new Repo_OITB();

                        //obtener el pedido de venta
                        Repo_OPED repoPed = new Repo_OPED(logger);
                        json = repoPed.Get(Convert.ToInt32(docgenerado.BaseEntry));
                        var pedido = JsonConvert.DeserializeObject<Documento>(json);

                        //preparar encabezado  --------------
                        Documento docProd = new Documento
                        {
                            DocEstado = "A",
                            DocTipo = 3010,
                            DocFecha = docgenerado.DocFecha,
                            SocioCode = docgenerado.SocioCode,
                            RazonSocial = docgenerado.RazonSocial,
                            VendedorCode = docgenerado.VendedorCode,
                            UsuarioCode = docgenerado.UsuarioCode,
                            Neto = pedido.Neto,
                            Iva = pedido.Iva,
                            Total = pedido.Total,
                            FechaRegistro = Convert.ToDateTime(docgenerado.FechaRegistro),
                            FechaIngresoPrep = Convert.ToDateTime(pedido.FechaRegistro),
                            Version = docgenerado.Version,
                            UsuarioNombre = docgenerado.UsuarioNombre,
                            BaseEntry = docgenerado.BaseEntry,
                            BodegaCode = config.BodegaProduccion,
                            BaseTipo = docgenerado.BaseTipo,
                            FechaEntrega = pedido.FechaEntrega,
                            RetiraCliente = docgenerado.RetiraCliente,
                            Lineas = new List<DocumentoLinea>(),
                            DireccionCode=pedido.DireccionCode,
                            CondicionCode=pedido.CondicionCode
                        };
                        //-----------------------------------

                        //generar detalle -----------
                        var listProduccion = new List<DocumentoLinea>();
                        foreach (var i in docgenerado.Lineas)
                        {
                            if (i.TipoCode == "B")
                            {
                                var det = pedido.Lineas.Find(x => x.ProdCode == i.ProdCode && x.DocLinea == i.BaseLinea);

                                json = repoStock.Get(i.ProdCode, i.BodegaCode);
                                var oitb = JsonConvert.DeserializeObject<OITB>(json);
                                if (oitb != null)
                                {
                                    decimal habian = Convert.ToDecimal(oitb.Stock) - (Convert.ToDecimal(oitb.Asignado) - Convert.ToDecimal(i.CantidadSolicitada));
                                    decimal enviaraproduccion = 0;
                                    if (true)
                                    {
                                        if (habian > 0)
                                        {
                                            enviaraproduccion = habian >= Convert.ToDecimal(i.CantidadSolicitada) ? 0 : Convert.ToDecimal(i.CantidadSolicitada) - habian;
                                        }
                                        else if (habian <= 0)
                                        {
                                            enviaraproduccion = Convert.ToDecimal(i.CantidadSolicitada);
                                        }
                                    }

                                    if (enviaraproduccion > 0)
                                    {
                                        docProd.Lineas.Add(new DocumentoLinea
                                        {
                                            BaseEntry = i.BaseEntry,
                                            BaseLinea = i.BaseLinea,
                                            BaseTipo = i.BaseTipo,
                                            DocTipo= docProd.DocTipo,

                                            CantidadPendiente = Convert.ToDecimal(enviaraproduccion),
                                            CantidadReal = 0,
                                            CantidadSolicitada = Convert.ToDecimal(enviaraproduccion),
                                            TotalSolicitado = Convert.ToDecimal(det.PrecioFinal) * enviaraproduccion,
                                            TotalReal = 0,
                                            BodegaCode = config.BodegaProduccion,

                                            Descuento = Convert.ToDecimal(det.Descuento),
                                            FechaConfirma = i.FechaConfirma,
                                            LineaEstado = "A", // i.LineaEstado,
                                            PrecioFinal = Convert.ToDecimal(det.PrecioFinal),
                                            PrecioUnitario = Convert.ToDecimal(det.PrecioUnitario),
                                            PrecioVolumen = Convert.ToDecimal(det.PrecioVolumen),
                                            Disponible = Convert.ToDecimal(det.Disponible),

                                            FactorPrecio = Convert.ToDecimal(det.FactorPrecio),
                                            ProdCode = i.ProdCode,
                                            ProdNombre = i.ProdNombre,
                                            UsuarioCodeConfirma = i.UsuarioCodeConfirma,
                                            FamiliaCode = det.FamiliaCode,
                                            AnimalCode = det.AnimalCode,
                                            Costo = det.Costo,
                                            FormatoVtaCode = det.FormatoVtaCode,
                                            Margen = det.Margen,
                                            Medida = det.Medida,
                                            TipoCode = det.TipoCode,
                                            LineaItem = det.LineaItem,
                                            Volumen = det.Volumen,

                                            MargenRegla = det.MargenRegla,
                                            AnimalNombre = det.AnimalNombre,
                                            FamiliaNombre = det.FamiliaNombre,
                                            FrmtoVentaNombre = det.FrmtoVentaNombre,
                                            MarcaCode = det.MarcaCode,
                                            MarcaNombre = det.MarcaNombre,
                                            OrigenCode = det.OrigenCode,
                                            OrigenNombre = det.OrigenNombre,
                                            RefrigeraCode = det.RefrigeraCode,
                                            RefrigeraNombre = det.RefrigeraNombre,
                                            SolicitadoAnterior = 0,
                                            CantidadEntregada = 0,
                                            Completado = 0

                                        });
                                    }
                                }
                            }
                        }

                        //3- generar orden de producción OPDC
                        if (docProd.Lineas.Any())
                        {
                            json = repoProd.Add(docProd);
                            var docprodgenerado = JsonConvert.DeserializeObject<Documento>(json);
                            if (docprodgenerado != null && docprodgenerado.Lineas != null && docprodgenerado.Lineas.Any())
                            {
                                if (docgenerado.DocumentoReferencia == null)
                                    docgenerado.DocumentoReferencia = new List<Documento>();

                                docgenerado.DocumentoReferencia.Add(docprodgenerado);
                            }
                        }
                    }
                    //----------------------------------------------------------

                }
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Picking Generado";
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

        public MensajeReturn Modify(string item)
        {
            MensajeReturn msg;

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);

            Repo_OPKG repo = new Repo_OPKG(logger);
            Repo_PKG1 repolin = new Repo_PKG1();
            var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            var linupdate = doc.Lineas;
            var opkg = JsonConvert.DeserializeObject<OPKG>(item);
            var json = "";
            json = repo.Get(doc.DocEntry);
            var opkgActual = JsonConvert.DeserializeObject<Documento>(json);
            var ped = new Documento();

            if (opkgActual.DocEstado == "C")
            {
                ped = opkgActual; // JsonConvert.DeserializeObject<Documento>(json);
                //ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Picking no existe" : "Actualizar Picking";
                msg.data = ped;

                return msg;
            }
            UpdateDetalle(linupdate, linoriginal);




            //realizar cierre si estado del picking = C--------------
            if (opkg.DocEstado == "C")
            {
                //actualizar detalle del pedido
                Repo_OPED repop = new Repo_OPED(logger);
                Repo_PED1 repod = new Repo_PED1(logger);
                Repo_PDC1 repodp = new Repo_PDC1();
                Repo_SMP1 reposm = new Repo_SMP1();
                Repo_PKG1 repok = new Repo_PKG1();
                Repo_OCFG_VTA repoVtn = new Repo_OCFG_VTA();
                json = repoVtn.Get(1);
                var param = JsonConvert.DeserializeObject<OCFG_VTA>(json);
                var tolerancia = param.Tolerancia;

                json = repok.List(opkg.DocEntry);
                var listPkg1 = JsonConvert.DeserializeObject<List<PKG1>>(json);
                var logcierre = "";
                if (opkg.BaseTipo == 10) //viene desde un pedido de ventas
                {
                    logcierre = CerrarPickingOPED(listPkg1, Convert.ToDecimal(tolerancia));
                    msg = new MensajeReturn();
                    if (logcierre != "")
                    {
                        opkg.DocEstado = "A";
                        repo.Modify(opkg);
                        msg.statuscode = HttpStatusCode.PreconditionFailed;
                        msg.error = true;
                        msg.msg = logcierre;
                        msg.data = "";
                        return msg;
                    }
                    else
                    {
                        json = repo.Modify(opkg);
                        ped = JsonConvert.DeserializeObject<Documento>(json);
                        ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.OK;
                        msg.error = false;
                        msg.msg = ped == null ? "Picking no existe" : "Actualizar Picking";
                        msg.data = ped;

                        return msg;
                    }
                }

                if (opkg.BaseTipo == 3010) //viene desde una orden de producción
                {
                    json = repok.List(opkg.DocEntry);
                    var list = JsonConvert.DeserializeObject<List<PKG1>>(json);
                    logcierre = "";
                    //--cerrar Desglose de cada linea PKG1 ---
                    //Generar Ajustes de inventario para item de producción ----
                    foreach (var pkg1 in list)
                    {
                        //25-09-2024: solo ajustar el producto principal, el desglose se realizara en nuevo formulario
                        //var cierre = CerrarPickingOPDC(pkg1, Convert.ToDecimal(tolerancia));
                        var cierre = CerrarPickingOPDC(pkg1);
                        if (cierre != "")
                        {
                            logcierre += String.Format("{0}\n", cierre.Replace("#",""));
                            break;
                        }
                        else
                        {
                            //cerrar la linea del pkg1, pkg2 y pk33
                            

                        }
                    }

                    if (logcierre.Replace("\n", "") != "") //existe un error
                    {
                        opkg.DocEstado = "A";
                        repo.Modify(opkg);

                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.PreconditionFailed;
                        msg.error = true;
                        msg.msg = logcierre;
                        msg.data = "";
                        return msg;
                    }
                    else
                    {
                        bool cerrarOPDC = true;
                        Repo_OPDC repoPrdc = new Repo_OPDC(logger);
                        json = repoPrdc.Get(Convert.ToInt32(opkg.BaseEntry));
                        var opdc = JsonConvert.DeserializeObject<OPDC>(json);

                        var repoPed = new Repo_OPED(logger);
                        var repoPedDet = new Repo_PED1(logger);

                        Documento oped = null;
                        if (opdc.BaseTipo == 10)
                        {
                            json = repoPed.Get(Convert.ToInt32(opdc.BaseEntry));
                            oped = JsonConvert.DeserializeObject<Documento>(json);
                        }


                        foreach (var d in listPkg1)
                        {
                            //(Tiene Receta) y OPKG.BaseTipo=3010 (viene de una Solicitud de Producción)
                            //----------------------------------------------------------
                            json = repodp.Get(Convert.ToInt32(d.BaseLinea));
                            var pdc1 = JsonConvert.DeserializeObject<PDC1>(json);
                            if (d.TieneReceta == null)
                                d.TieneReceta = "N";

                            if (d.TieneReceta == "N" && d.KilosPorCaja > d.Diferencia && d.Tolerancia <= tolerancia)
                            {
                                pdc1.CantidadPendiente = 0;
                            }
                            else if (d.TieneReceta == "Y" && d.Tolerancia <= tolerancia)
                            {
                                pdc1.CantidadPendiente = 0;
                            }
                            pdc1.CantidadReal += d.CantidadReal;
                            pdc1.TotalReal = pdc1.CantidadReal * pdc1.PrecioFinal;
                            pdc1.CantidadPendiente -= pdc1.CantidadReal;
                            pdc1.Completado = pdc1.CantidadPendiente <= 0 ? 1 : pdc1.CantidadReal / pdc1.CantidadPendiente;
                            pdc1.LineaEstado = pdc1.Completado >= 1 ? "C" : "A";

                            if (cerrarOPDC)
                            {
                                cerrarOPDC = pdc1.Completado < 1 ? false : true;
                            }
                            //actualizar pdc1
                            repodp.Modify(pdc1);
                            //*************************************************************
                            //16-10-2024: marcar el pedido de venta relacionado si aplica
                            //comprobar si el BaseTipo del OPDC es un pedido de venta
                            if (pdc1.BaseTipo == 10 && oped!=null && oped.DocEstado!="C" && oped.Lineas!=null && oped.Lineas.Any())
                            {
                                var ped1 = oped.Lineas.Find(x => x.DocLinea == pdc1.BaseLinea && x.DocEntry == pdc1.BaseEntry && x.DocTipo == pdc1.BaseTipo);
                                if(ped1!=null)
                                {
                                    //sumar cantidad marcada al item del pedido de ventas
                                    ped1.CantidadReal += d.CantidadReal;
                                    ped1.TotalReal = ped1.PrecioFinal * ped1.CantidadReal;
                                    ped1.Completado = ped1.CantidadPendiente <= 0 ? 1 : ped1.CantidadReal / ped1.CantidadPendiente;
                                    ped1.EnProduccion = pdc1.LineaEstado =="C" || ped1.Completado>=1 ? null : ped1.EnProduccion;
                                    json = JsonConvert.SerializeObject(ped1);
                                    var det = JsonConvert.DeserializeObject<PED1>(json);
                                    repoPedDet.Modify(det);
                                }
                            }
                        }
                        if (cerrarOPDC)
                        {
                            opdc.DocEstado = "C";
                            repoPrdc.Modify(opdc);
                        }
                        json = repo.Modify(opkg);
                        ped = JsonConvert.DeserializeObject<Documento>(json);
                        ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.OK;
                        msg.error = false;
                        msg.msg = ped == null ? "Picking no existe" : "Actualizar Picking";
                        msg.data = ped;

                        return msg;
                    }
                }

                if (opkg.BaseTipo == 16) //viene desde una solicitud de materia prima
                {
                    foreach (var pkg1 in listPkg1)
                    {
                        var cierre = CerrarPickingOSMP(pkg1, Convert.ToDecimal(tolerancia));
                        if (cierre != "")
                        {
                            logcierre += String.Format("{0}\n", cierre.Replace("#", ""));
                        }

                    }

                    //logcierre = CerrarPickingOSMP(listk, Convert.ToDecimal(tolerancia));
                    msg = new MensajeReturn();
                    if (logcierre.Replace("\n", "") != "")
                    {
                        opkg.DocEstado = "A";
                        repo.Modify(opkg);
                        msg.statuscode = HttpStatusCode.PreconditionFailed;
                        msg.error = true;
                        msg.msg = logcierre;
                        msg.data = "";
                        return msg;
                    }
                    else
                    {
                        bool cerrarOSMP = true;
                        foreach (var d in listPkg1)
                        {

                            //(Tiene Receta) y OPKG.BaseTipo=3010 (viene de una Solicitud de Producción)
                            //----------------------------------------------------------
                            json = reposm.Get(Convert.ToInt32(d.BaseLinea));
                            var smp1 = JsonConvert.DeserializeObject<SMP1>(json);
                            if (d.TieneReceta == null)
                                d.TieneReceta = "N";

                            if (d.TieneReceta == "N" && d.KilosPorCaja > d.Diferencia && d.Tolerancia <= tolerancia)
                            {
                                smp1.CantidadPendiente = 0;
                            }
                            else if (d.TieneReceta == "Y" && d.Tolerancia <= tolerancia)
                            {
                                smp1.CantidadPendiente = 0;
                            }
                            smp1.CantidadReal += d.CantidadReal;
                            smp1.Completado = smp1.CantidadPendiente == 0 ? 1 : smp1.CantidadReal / smp1.CantidadPendiente;
                            smp1.TotalReal = smp1.CantidadReal * smp1.PrecioFinal;
                            smp1.LineaEstado = smp1.Completado == 1 ? "C" : "A";
                            if (cerrarOSMP)
                            {
                                cerrarOSMP = smp1.Completado < 1 ? false : true;
                            }

                            reposm.Modify(smp1);
                        }
                        if (cerrarOSMP)
                        {
                            Repo_OSMP repoPrdc = new Repo_OSMP(logger);
                            json = repoPrdc.Get(Convert.ToInt32(opkg.BaseEntry));
                            var osmp = JsonConvert.DeserializeObject<OSMP>(json);
                            osmp.DocEstado = "C";
                            repoPrdc.Modify(osmp);
                        }

                        json = repo.Modify(opkg);
                        ped = JsonConvert.DeserializeObject<Documento>(json);
                        ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.OK;
                        msg.error = false;
                        msg.msg = ped == null ? "Picking no existe" : "Actualizar Picking";
                        msg.data = ped;

                        return msg;
                    }
                }
            }
            //-------------------------------------------------------
             
            json = repo.Modify(opkg);
            ped = JsonConvert.DeserializeObject<Documento>(json);
            ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.msg = ped == null ? "Picking no existe" : "Actualizar Picking";
            msg.data = ped;

            return msg;
        }

        private string CerrarPickingOPED(List<PKG1> list, decimal tolerancia)
        {
            try
            {
                Repo_PED1 repod = new Repo_PED1(logger);
                foreach (var d in list)
                {
                    var json = repod.Get(Convert.ToInt32(d.BaseLinea));
                    var lin = JsonConvert.DeserializeObject<PED1>(json);
                    if (lin.CantidadReal == null)
                        lin.CantidadReal = 0;

                    lin.CantidadReal += d.CantidadReal;
                    lin.TotalReal = lin.CantidadReal * lin.PrecioFinal;

                    if (d.TieneReceta == null)
                        d.TieneReceta = "N";

                    if (d.TieneReceta == "N" && (d.KilosPorCaja > d.Diferencia || d.Tolerancia <= tolerancia))
                    {
                        //lin.CantidadPendiente = 0;
                        lin.CantidadPendiente = lin.CantidadReal;
                        lin.Completado = 1;
                        d.Completado = 1;
                        //lin.LineaEstado = "C";
                    }
                    else if (d.TieneReceta == "Y" && d.Tolerancia <= tolerancia)
                    {
                        lin.CantidadPendiente = lin.CantidadReal;
                        lin.Completado = 1;
                        d.Completado = 1;
                        //lin.LineaEstado = "C";
                    }
                    else
                    {
                        lin.Completado = lin.CantidadPendiente == 0 ? 1 : lin.CantidadReal / lin.CantidadPendiente;
                    }
                    repod.Modify(lin);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string CerrarPickingOPDC(PKG1 pkg1)
        {
            MensajeReturn msg;
            try
            {
                

                //obtener el producto principal
                Repo_OITM repoCosto = new Repo_OITM(logger);
                ManagerAjustes mngAJ = new ManagerAjustes(logger, cnndf);

                //construir Transaccion inventario INGRESO----
                var trxi = new Transaccion { Lineas = new List<TransaccionLinea>() };
                trxi.DocEntry = 0;
                trxi.DocEstado = "A";
                trxi.DocTipo = 17;
                trxi.TipoAjuste = 402; // ingreso
                trxi.DocFecha = DateTime.Now.Date;
                trxi.BodegaCodeDestino = "TOLEDO"; //  pkg1.BodegaCode;
                trxi.BodegaCodeOrigen = "";
                trxi.FechaRegistro = DateTime.Now;

                var json = repoCosto.Get(pkg1.ProdCode);
                var producto = JsonConvert.DeserializeObject<OITM>(json);
                if (pkg1.CantidadReal > 0)
                {
                    trxi.Lineas.Add(new TransaccionLinea
                    {
                        ProdCode = pkg1.ProdCode,
                        ProdNombre = pkg1.ProdNombre,
                        CantidadSolicitada = Convert.ToDecimal(pkg1.CantidadReal),
                        Costo = Convert.ToDecimal(producto.Costo),
                        MedidaCode = producto.Medida
                    });
                }

                if (trxi.Lineas.Any())
                {
                    msg = mngAJ.Add(trxi);
                    if (!msg.error)
                    {
                        return "";
                    }
                    else
                    {
                        return msg.msg.Replace("#", "").Replace("\n", "");
                    }
                }
                else
                {
                    return "";
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        private string CerrarPickingOPDC(PKG1 pkg1, decimal tolerancia)
        {
            MensajeReturn msg;
            try
            {
                //recuperar desglose
                Repo_PKG2 repodesglose = new Repo_PKG2(logger);
                var json = repodesglose.GetBase(pkg1.DocEntry, pkg1.DocLinea);
                var desglose = JsonConvert.DeserializeObject<Documento>(json);
                var pkg2 = JsonConvert.DeserializeObject<PKG2>(json);

                Repo_OITM repoCosto = new Repo_OITM(logger);
                ManagerAjustes mngAJ = new ManagerAjustes(logger, cnndf);

                if (desglose != null)
                {
                    //primero realizar ajuste de salida del producto Alternativo en Producccion
                    var listAlternativo = desglose.Lineas.FindAll(x => x.ProdTipo == "Alternativo");
                    if (listAlternativo.Any())
                    {
                        //construir Transaccion inventario EGRESO--------------------------------
                        var trxe = new Transaccion { Lineas = new List<TransaccionLinea>() };
                        trxe.DocEntry = 0;
                        trxe.DocEstado = "A";
                        trxe.DocTipo = 17;
                        trxe.TipoAjuste = 401; // egreso
                        trxe.DocFecha = DateTime.Now.Date;
                        trxe.BodegaCodeDestino = "";
                        trxe.BodegaCodeOrigen = listAlternativo[0].BodegaCode;// b.BodegaCode;
                        trxe.FechaRegistro = DateTime.Now;
                        foreach (var d in listAlternativo)
                        {
                            if (d.CantidadReal > 0)
                            {
                                json = repoCosto.Get(d.ProdCode);
                                var producto = JsonConvert.DeserializeObject<OITM>(json);
                                trxe.Lineas.Add(new TransaccionLinea
                                {
                                    ProdCode = d.ProdCode,
                                    ProdNombre = d.ProdNombre,
                                    CantidadSolicitada = Convert.ToDecimal(d.CantidadReal),
                                    Costo = Convert.ToDecimal(producto.Costo),
                                    MedidaCode = producto.Medida
                                });
                            }
                        }
                        if (trxe.Lineas.Any())
                        {
                            //realizar ajuste de salida en Producción ---
                            msg = mngAJ.Add(trxe);
                            //-------------------------------------------
                            if (!msg.error)
                            {
                                //segundo realizar ajuste de entrada del producto Principal en Toledo ---
                                var listPrincipal = desglose.Lineas.FindAll(x => x.ProdTipo == "Principal");
                                if (listPrincipal.Any())
                                {
                                    //construir Transaccion inventario INGRESO----
                                    var trxi = new Transaccion { Lineas = new List<TransaccionLinea>() };
                                    trxi.DocEntry = 0;
                                    trxi.DocEstado = "A";
                                    trxi.DocTipo = 17;
                                    trxi.TipoAjuste = 402; // ingreso
                                    trxi.DocFecha = DateTime.Now.Date;
                                    trxi.BodegaCodeDestino = listPrincipal[0].BodegaCode;
                                    trxi.BodegaCodeOrigen = "";
                                    trxi.FechaRegistro = DateTime.Now;
                                    foreach (var d in listPrincipal)
                                    {
                                        json = repoCosto.Get(d.ProdCode);
                                        var producto = JsonConvert.DeserializeObject<OITM>(json);
                                        if (pkg1.CantidadReal > 0)
                                        {
                                            trxi.Lineas.Add(new TransaccionLinea
                                            {
                                                ProdCode = d.ProdCode,
                                                ProdNombre = d.ProdNombre,
                                                CantidadSolicitada = Convert.ToDecimal(pkg1.CantidadReal),
                                                Costo = Convert.ToDecimal(producto.Costo),
                                                MedidaCode = producto.Medida
                                            });
                                        }
                                    }
                                    //17-09-2024: Ajustes de productos Desglose se realizará en nuevo formulario, --------------
                                    //ahora solo se ajusta el producto alterntivo y el picking se debe cerrar

                                    ////tercero realizar ajuste de entrada de los productos Desgrlose en Produccion ---
                                    //var listDesglose = desglose.Lineas.FindAll(x => x.ProdTipo == "Desglose");
                                    //foreach (var d in listDesglose)
                                    //{
                                    //    json = repoCosto.Get(d.ProdCode);
                                    //    var producto = JsonConvert.DeserializeObject<OITM>(json);
                                    //    if (d.CantidadReal > 0)
                                    //    {
                                    //        trxi.Lineas.Add(new TransaccionLinea
                                    //        {
                                    //            ProdCode = d.ProdCode,
                                    //            ProdNombre = d.ProdNombre,
                                    //            CantidadSolicitada = Convert.ToDecimal(d.CantidadReal),
                                    //            Costo = Convert.ToDecimal(producto.Costo),
                                    //            MedidaCode = producto.Medida
                                    //        });
                                    //    }
                                    //}
                                    //-------------------------------------------------------------------------------------------
                                    if (trxi.Lineas.Any())
                                    {
                                        msg = mngAJ.Add(trxi);
                                        if (!msg.error)
                                        {
                                            //cerrar pkg2
                                            if (pkg2 != null)
                                            {
                                                pkg2.DocEstado = "C";
                                                repodesglose.Modify(pkg2);
                                            }
                                            return "";
                                        }
                                        else
                                        {
                                            return msg.msg.Replace("#", "").Replace("\n", "");
                                        }
                                    }
                                    else
                                    {
                                        return "No existen productos a marcar";
                                    }
                                    //-------------------------------------------------------------------------------
                                }
                                else
                                {
                                    return "No existen productos a marcar";
                                }
                                //-----------------------------------------------------------------------
                            }
                            else
                            {
                                return msg.msg.Replace("#", "").Replace("\n", "");
                            }
                        }
                        else
                        {
                            //cerrar pkg2
                            pkg2.DocEstado = "C";
                            repodesglose.Modify(pkg2);
                            //return "";
                            logger.Info("Pedido: {0} Picking: {1} No existen productos marcados, Picking queda cerrado", pkg1.BaseEntry, pkg1.DocEntry);
                            return "";
                        }
                    }
                    else
                    {
                        return "No existen productos a marcar";
                    }
                }
                else
                {
                    return "";
                }
                //-----------------------------------------------------------------------



                ////-agrupar por bodega---
                //var bodegaList = desglose.Lineas
                //        .GroupBy(u => u.BodegaCode)
                //        .Select(grp => grp.ToList())
                //        .ToList();

                //foreach (var grupo in bodegaList)
                //{
                //    foreach (var b in grupo)
                //    {
                //        var listRegistrosbodega = desglose.Lineas.FindAll(x => x.BodegaCode == b.BodegaCode);
                //        var listPrincipal = listRegistrosbodega.FindAll(x => x.ProdTipo == "Principal");
                //        var listAlternativo = listRegistrosbodega.FindAll(x => x.ProdTipo == "Alternativo");
                //        var listDesglose = listRegistrosbodega.FindAll(x => x.ProdTipo == "Desglose");

                //        //construir Transaccion inventario EGRESO----
                //        var trxe = new Transaccion { Lineas = new List<TransaccionLinea>() };
                //        trxe.DocEntry = 0;
                //        trxe.DocEstado = "A";
                //        trxe.DocTipo = 401; // egreso
                //        trxe.DocFecha = DateTime.Now.Date;
                //        trxe.BodegaCodeDestino = "";
                //        trxe.BodegaCodeOrigen = b.BodegaCode;
                //        trxe.FechaRegistro = DateTime.Now;
                //        foreach (var d in listAlternativo)
                //        {
                //            json = repoCosto.Get(d.ProdCode);
                //            var producto = JsonConvert.DeserializeObject<OITM>(json);
                //            if (d.CantidadReal > 0)
                //                {
                //                trxe.Lineas.Add(new TransaccionLinea
                //                {
                //                    ProdCode = d.ProdCode,
                //                    ProdNombre = d.ProdNombre,
                //                    Cantidad = Convert.ToDecimal(d.CantidadReal),
                //                    Costo = Convert.ToDecimal(producto.Costo),
                //                    MedidaCode = producto.Medida
                //                });
                //            }
                //        }

                //        ManagerProductos mngPrdcto = new ManagerProductos(logger, cnndf);
                //        if (trxe.Lineas.Any())
                //        {
                //            msg = mngPrdcto.AjusteInventario(trxe);
                //        }
                //        else
                //        {
                //            msg = new MensajeReturn { error = false };
                //        }
                //        if (!msg.error)
                //        {
                //            //construir Transaccion inventario INGRESO----
                //            var trxi = new Transaccion { Lineas = new List<TransaccionLinea>() };
                //            trxi.DocEntry = 0;
                //            trxi.DocEstado = "A";
                //            trxi.DocTipo = 402; // ingreso
                //            trxi.DocFecha = DateTime.Now.Date;
                //            trxi.BodegaCodeDestino = b.BodegaCode;
                //            trxi.BodegaCodeOrigen = "";
                //            trxi.FechaRegistro = DateTime.Now;
                //            foreach (var d in listPrincipal)
                //            {
                //                json = repoCosto.Get(d.ProdCode);
                //                var producto = JsonConvert.DeserializeObject<OITM>(json);
                //                if (pkg1.CantidadReal > 0)
                //                {
                //                    trxi.Lineas.Add(new TransaccionLinea
                //                    {
                //                        ProdCode = d.ProdCode,
                //                        ProdNombre = d.ProdNombre,
                //                        Cantidad = Convert.ToDecimal(pkg1.CantidadReal),
                //                        Costo = Convert.ToDecimal(producto.Costo),
                //                        MedidaCode = producto.Medida
                //                    });
                //                }
                //            }
                //            foreach (var d in listDesglose)
                //            {
                //                json = repoCosto.Get(d.ProdCode);
                //                var producto = JsonConvert.DeserializeObject<OITM>(json);
                //                if (d.CantidadReal > 0)
                //                {
                //                    trxi.Lineas.Add(new TransaccionLinea
                //                    {
                //                        ProdCode = d.ProdCode,
                //                        ProdNombre = d.ProdNombre,
                //                        Cantidad = Convert.ToDecimal(d.CantidadReal),
                //                        Costo = Convert.ToDecimal(producto.Costo),
                //                        MedidaCode = producto.Medida
                //                    });
                //                }
                //            }

                //            if (trxi.Lineas.Any())
                //            {
                //                msg = mngPrdcto.AjusteInventario(trxi);
                //            }
                //            else
                //            {
                //                msg = new MensajeReturn { error = false };
                //            }

                //            if (!msg.error)
                //            {
                //                //return "";
                //            }
                //            else
                //            {
                //                return msg.msg;
                //            }
                //        }
                //        else
                //        {
                //            return msg.msg;
                //        }
                //    }
                //}
                ////--------------------------------------------------------
                //return "";
            }
            catch (Exception ex)
            {
                logger.Error("se ha generado un error al CerrarPickingOPDC({1}): {0}", ex.Message, pkg1.DocEntry);
                logger.Error("se ha generado un error al CerrarPickingOPDC({1}): {0}", ex.StackTrace, pkg1.DocEntry);
                return ex.Message;
            }
        }

        private string CerrarPickingOSMP(PKG1 pkg1, decimal tolerancia)
        {
            MensajeReturn msg;
            try
            {
                //recuperar desglose
                Repo_PKG2 repodesglose = new Repo_PKG2(logger);
                var json = repodesglose.GetBase(pkg1.DocEntry, pkg1.DocLinea);
                var desglose = JsonConvert.DeserializeObject<Documento>(json);
                var pkg2 = JsonConvert.DeserializeObject<PKG2>(json);
                if (desglose != null && desglose.DocEstado=="A")
                {
                    Repo_SMP1 repod = new Repo_SMP1();
                    var trxe = new Transaccion { Lineas = new List<TransaccionLinea>() };
                    trxe.DocEntry = 0;
                    trxe.DocEstado = "A";
                    trxe.DocTipo = 17; 
                    trxe.TipoAjuste = 403; // traslado
                    trxe.DocFecha = DateTime.Now.Date;
                    trxe.BodegaCodeOrigen = "TOLEDO";
                    trxe.BodegaCodeDestino = "PRODUCCION";
                    trxe.FechaRegistro = DateTime.Now;
                    Repo_OITM repoCosto = new Repo_OITM(logger);
                    ManagerAjustes mngAJ = new ManagerAjustes(logger, cnndf);

                    //se debe hacer un traslado desde Toledo a Producción solo de los productos Alternativos
                    var listAlternativo = desglose.Lineas.FindAll(x => x.ProdTipo == "Alternativo");

                    foreach (var d in listAlternativo)
                    {
                        if (d.CantidadReal > 0)
                        {
                            json = repoCosto.Get(d.ProdCode);
                            var producto = JsonConvert.DeserializeObject<OITM>(json);
                            trxe.Lineas.Add(new TransaccionLinea
                            {
                                ProdCode = d.ProdCode,
                                ProdNombre = d.ProdNombre,
                                CantidadSolicitada = Convert.ToDecimal(d.CantidadReal),
                                Costo = Convert.ToDecimal(producto.Costo),
                                MedidaCode = producto.Medida
                            });
                        }
                    }
                    if (trxe.Lineas.Any())
                    {
                        //realizar traslado a Producción --------
                        msg = mngAJ.Add(trxe);
                        if (!msg.error)
                        {
                            //cerrar pkg2
                            pkg2.DocEstado = "C";
                            repodesglose.Modify(pkg2);
                            return "";
                        }
                        else
                        {
                            return msg.msg;
                        }
                        //---------------------------------------
                    }
                    else
                    {
                        return "No existen productos a marcar";
                    }

                }
                else
                {
                    return "";
                }
            }

            catch (Exception ex)
            {
                logger.Error("se ha generado un error al CerrarPickingOSMP({1}): {0}", ex.Message, pkg1.DocEntry);
                logger.Error("se ha generado un error al CerrarPickingOSMP({1}): {0}", ex.StackTrace, pkg1.DocEntry);
                return ex.Message;
            }
        }

        public MensajeReturn List(string bodegacode, string estado)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);
            var json = repo.List(bodegacode, estado);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Bandeja Bodega";
            msg.data = list;
            return msg;
        }

        public MensajeReturn ListPicking(string bodegacode, string estado)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);
            var json = repo.ListPicking(bodegacode, estado);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Bandeja " + bodegacode;
            msg.data = list;
            return msg;
        }

        public MensajeReturn Search(string palabras, string bodegacode)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);

            var json = repo.Search(palabras, bodegacode);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Pedidos Bodega";
            msg.data = list;
            return msg;
        }

        public MensajeReturn Get(int docentry)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);

            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = doc == null ? 0 : 1;
            msg.msg = doc == null ? "Picking no existe" : "Picking";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn GetBase(int baseentry)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);

            var json = repo.GetBase(baseentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = doc == null ? 0 : 1;
            msg.msg = doc == null ? "Picking no existe" : "Picking";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn GetBaseLinea(int baseentry, int baselinea)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);

            var json = repo.GetBaseLinea(baseentry, baselinea);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = doc == null ? 0 : 1;
            msg.msg = doc == null ? "Picking no existe" : "Picking";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn Propiedades()
        {
            Repo_OPER repopr = new Repo_OPER();
            var prop = new DocumentoPropiedades();

            var json = repopr.List(); prop.operadores = JsonConvert.DeserializeObject<List<OPER>>(json);

            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = prop.operadores.Count;
            msg.msg = "Propiedades Picking";
            msg.data = prop;
            return msg;
        }
        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_PKG1 repo = new Repo_PKG1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<PKG1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PKG1>(json);
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
                            var lin = JsonConvert.DeserializeObject<PKG1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<PKG1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PKG1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }


        #region Desglose
        public MensajeReturn AddDesglose(Documento doc)
        {
            MensajeReturn msg;
            try
            {
                Repo_PKG2 repo = new Repo_PKG2(logger);
                var json = repo.Add(doc);
                var docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Picking Generado";
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

        public MensajeReturn GetDesglose(int docentry)
        {
            Repo_PKG2 repo = new Repo_PKG2(logger);

            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = doc == null ? 0 : 1;
            msg.msg = doc == null ? "Picking Desglose no existe" : "Picking Desglose";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn Picking(int docentry, string origen)
        {
            Repo_OPKG repo = new Repo_OPKG(logger);
            string json = repo.Picking(docentry, origen);
            Picking picking = JsonConvert.DeserializeObject<Picking>(json);
            
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = "Picking";
            msg.data = picking;
            return msg;
        }

        public MensajeReturn ModifyDesglose(string item)
        {

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);

            Repo_PKG2 repo = new Repo_PKG2(logger);
            Repo_PKG3 repolin = new Repo_PKG3();
            var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            var linupdate = doc.Lineas;

            UpdateDetalleDesglose(linupdate, linoriginal);
            var oped = JsonConvert.DeserializeObject<PKG2>(item);

            var json = repo.Modify(oped);
            var ped = JsonConvert.DeserializeObject<Documento>(json);
            ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.msg = ped == null ? "Pedido no existe" : "Actualizar Pedido";
            msg.data = ped;

            return msg;
        }

        private void UpdateDetalleDesglose(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_PKG3 repo = new Repo_PKG3();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<PKG3>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PKG3>(json);
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
                            var lin = JsonConvert.DeserializeObject<PKG3>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<PKG3>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PKG3>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
        #endregion Desglose

    }
}
