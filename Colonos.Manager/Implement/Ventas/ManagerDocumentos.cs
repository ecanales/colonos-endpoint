using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.Entidades;
using Newtonsoft.Json;
using NLog;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Colonos.Entidades.Defontana;
using System.Configuration;
using Colonos.Entidades.Drivin;

namespace Colonos.Manager
{
    public class ManagerDocumentos
    {
        Logger logger;
        cnnDF cnndf;
        cnnDrivin cnndrivin;

        public ManagerDocumentos(Logger _logger, cnnDF _cnndf, cnnDrivin _cnndrivin)
        {
            logger = _logger;
            cnndf = _cnndf;
            cnndrivin = _cnndrivin;
        }

        public MensajeReturn Add(string item)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {

                var input = JsonConvert.DeserializeObject<Documento>(item);
                if (input != null)
                {
                    if (input.DocTipo != 2110 && input.DocTipo != 15 && input.DocTipo != 16 && input.DocTipo != 3010 && input.DocTipo != 18)
                    {
                        //validar si cliente no este betado ---
                        Repo_OSCP repocli = new Repo_OSCP(logger);
                        var repoEst = new Repo_SCP7();
                        var json = repoEst.List();
                        var estados = JsonConvert.DeserializeObject<List<SCP7>>(json);
                        decimal creditodisponible = 0;

                        json = repocli.Get(input.SocioCode);
                        var cli = JsonConvert.DeserializeObject<OSCP>(json);
                        if (cli != null)
                        {
                            creditodisponible = cli.CreditoAutorizado ?? 0 - cli.CreditoUtiliado ?? 0;
                            var estado = estados.Find(x => x.EstadoOperativo == cli.EstadoOperativo);

                            if (estado.Descripcion == "BETADO")
                            {
                                msg = new MensajeReturn();
                                msg.error = true;
                                msg.statuscode = HttpStatusCode.BadRequest;
                                msg.msg = "Cliento no autorizado";
                                msg.data = JsonConvert.SerializeObject(estado);
                                logger.Error("mensaje: {0}. Data: {1}", msg.msg, msg.data);
                                return msg;
                            }
                        }
                        else
                        {
                            msg = new MensajeReturn();
                            msg.error = true;
                            msg.statuscode = HttpStatusCode.BadRequest;
                            msg.msg = String.Format("Cliento {0} no existe", input.SocioCode);
                            logger.Error("mensaje: {0}. Data: {1}", msg.msg, msg.data);
                            return msg;
                        }
                    }
                }
                if (true)
                {
                    ManagerPicking mngPK;
                    switch (input.DocTipo) 
                    {
                        case 10: //Nota de Pedido
                            ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                            msg = mngNP.Add(input);
                            break;
                        case 11: //Guia de despacho
                            break;
                        case 12: //Factura
                            ManagerFacturas mngFAC = new ManagerFacturas(logger, cnndf);
                            msg = mngFAC.Add(input);
                            break;
                        case 13: //Nota de Credito
                            break;
                        case 14: //Ruta OLOG
                            ManagerLogistica mngLOG = new ManagerLogistica(logger, cnndrivin);
                            msg = mngLOG.Add(input);
                            break;
                        case 15: //Ruta Drivin
                            ManagerRutas mngRUT = new ManagerRutas(logger, cnndrivin);
                            var inputruta = JsonConvert.DeserializeObject<DocumentoRuta>(item);
                            msg = mngRUT.Add(inputruta);
                            break;
                        case 2010: //Picking
                            mngPK = new ManagerPicking(logger, cnndf);
                            msg = mngPK.Add(input);
                            break;
                        case 2110: //Picking Desglose
                            mngPK = new ManagerPicking(logger, cnndf);
                            msg = mngPK.AddDesglose(input);
                            break;
                        case 16: //Solicitud de materia prima
                            ManagerSolicitudMP mngMP = new ManagerSolicitudMP(logger);
                            msg = mngMP.Add(input);
                            break;
                        case 3010: //solicitud de producción
                            ManagerProduccion mngPD = new ManagerProduccion(logger);
                            msg = mngPD.Add(input);
                            break;
                        case 17: //ajuste de inventario
                            ManagerAjustes mngAJ = new ManagerAjustes(logger, cnndf);
                            msg = mngAJ.Add(input);
                            break;
                        case 18: //Rendición produccion
                            ManagerRendicion mngRend = new ManagerRendicion(logger);
                            msg = mngRend.Add(input);
                            break;
                    }
                }

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
        
        public MensajeReturn ModifyEstadoItem(string item)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                var input = JsonConvert.DeserializeObject<Documento>(item);
                switch (input.DocTipo)
                {
                    case 10:
                        ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                        msg = mngNP.ModifyEstadoItem(item);
                        break;
                    case 3010: //solictud mataria prima
                        ManagerProduccion mngPD = new ManagerProduccion(logger);
                        msg = mngPD.ModifyEstadoItem(item);
                        break;
                }
                
                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn ModifyEtiqueta(string item)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                var input = JsonConvert.DeserializeObject<Documento>(item);
                ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                msg = mngNP.ModifyEtiqueta(item);
                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn ModifyOrdendeCompra(string item)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                var input = JsonConvert.DeserializeObject<Documento>(item);
                ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                msg = mngNP.ModifyOrdendeCompra(item);
                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn ModifySets(string item)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                var input = JsonConvert.DeserializeObject<List<Documento>>(item);

                switch (input[0].DocTipo)
                {
                    case 14: //Ruta Logistica
                        ManagerLogistica mngLOG = new ManagerLogistica(logger, cnndrivin);
                        msg = mngLOG.ModifySets(item);
                        break;
                }

                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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
            MensajeReturn msg = new MensajeReturn();
            try
            {
                var input = JsonConvert.DeserializeObject<Documento>(item);
                ManagerPicking mngPK;
                switch (input.DocTipo)
                {
                    case 10: //Nota de Pedido
                        ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                        msg = mngNP.Modify(item);
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        ManagerFacturas mngFAC = new ManagerFacturas(logger, cnndf);
                        msg = mngFAC.Modify(item);
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 14: //Ruta Logistica
                        ManagerLogistica mngLOG = new ManagerLogistica(logger, cnndrivin);
                        msg = mngLOG.Modify(item);
                        break;
                    case 2010: //Picking
                        mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.Modify(item);
                        break;
                    case 2110: //Picking Desglose
                        mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.ModifyDesglose(item);
                        break;
                    case 16: //solictud mataria prima
                        ManagerSolicitudMP mngMP = new ManagerSolicitudMP(logger);
                        msg = mngMP.Modify(item);
                        break;
                    case 3010: //solictud mataria prima
                        ManagerProduccion mngPD = new ManagerProduccion(logger);
                        msg = mngPD.Modify(item);
                        break;

                }

                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn Get(int docentry, int doctipo)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                ManagerPicking mngPK;
                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                        msg = mngNP.Get(docentry);
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        ManagerFacturas mngFAC = new ManagerFacturas(logger, cnndf);
                        msg = mngFAC.Get(docentry);
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 14: //Ruta Logistica
                        ManagerLogistica mngLOG = new ManagerLogistica(logger, cnndrivin);
                        msg = mngLOG.Get(docentry);
                        break;
                    case 2010: //Picking
                        mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.Get(docentry);
                        break;
                    case 2110: //Picking Desglose
                        mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.GetDesglose(docentry);
                        break;
                    case 3010: //Solicitud de produccion
                        var mngProd = new ManagerProduccion(logger);
                        msg = mngProd.Get(docentry);
                        break;
                    case 16: //solcitud mataria prima
                        ManagerSolicitudMP mngMP = new ManagerSolicitudMP(logger);
                        msg = mngMP.Get(docentry);
                        break;
                    case 18: //solcitud mataria prima
                        ManagerRendicion mngRend = new ManagerRendicion(logger);
                        msg = mngRend.Get(docentry);
                        break;
                    case 4015: //custodia
                        ManagerCustodia mngCus = new ManagerCustodia(logger, cnndrivin);
                        msg = mngCus.Get(docentry);
                        break;
                }

                //**********************************

                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn GetBase(int baseentry, int doctipo)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 14: //Ruta Logistica
                        break;
                    case 2010: //Picking
                        break;
                    case 2110: //Picking
                        ManagerPicking mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.GetBase(baseentry);
                        break;
                }

                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn GetBaseLinea(int baseentry, int doctipo, int baselinea)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 14: //Ruta Logistica
                        break;
                    case 2010: //Picking
                        break;
                    case 2110: //Picking
                        ManagerPicking mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.GetBaseLinea(baseentry, baselinea);
                        break;
                }

                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn Delete(int docentry)
        {
            try
            {
                Repo_OPED repo = new Repo_OPED(logger);
                Repo_PED1 repolin = new Repo_PED1(logger);

                
                using (TransactionScope transaction = new TransactionScope())
                {
                    var item = repolin.Delete(docentry);
                    item = repo.Delete(docentry);
                    transaction.Complete();

                    MensajeReturn msg = new MensajeReturn();
                    msg.statuscode = item ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                    msg.error = item;
                    msg.msg = !item ? "Pedido no existe" : "Pedido";
                    msg.data = "";
                   
                    return msg;

                   
                }

                
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

        public MensajeReturn Search(string palabras, string code, int doctipo, string usuario)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {

                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                        msg = mngNP.Search(palabras, code, usuario);
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        ManagerFacturas mngFAC = new ManagerFacturas(logger, cnndf);
                        msg = mngFAC.Search(palabras, code);
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 14: //Ruta Logistica
                        ManagerLogistica mngLOG = new ManagerLogistica(logger, cnndrivin);
                        msg = mngLOG.Search(palabras, code);
                        break;
                    case 2010: //Picking
                        ManagerPicking mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.Search(palabras, code);
                        break;
                    case 16: //solcitud materia prima
                        ManagerSolicitudMP mngMP = new ManagerSolicitudMP(logger);
                        msg = mngMP.Search(palabras, code);
                        break;
                    case 3010: //solcitud producción
                        ManagerProduccion  mngPD = new ManagerProduccion(logger);
                        msg = mngPD.Search(palabras, code);
                        break;

                }

                
                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn ListPicking(string code, string estado)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                ManagerPicking mngPK = new ManagerPicking(logger, cnndf);
                msg = mngPK.ListPicking(code, estado);
                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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

        public MensajeReturn List(int doctipo, string bodegacode="", string estadooperativo="", string vendedorcode="",  string estado="", int pendiente=-1, string fechaini="", string fechafin = "", string usuario="")
        {
            MensajeReturn msg = new MensajeReturn();
            estado = estado ?? "";
            try
            {
                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                            msg = mngNP.List(vendedorcode,estadooperativo,estado,pendiente,fechaini, fechafin, bodegacode,usuario );
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        ManagerFacturas mngFAC = new ManagerFacturas(logger,cnndf);
                        //if (estado == "")
                        //    msg = mngFAC.List();
                        //else
                            msg = mngFAC.List(estado, fechaini, fechafin);
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 14: //Ruta Logistica
                        ManagerLogistica mngLOG = new ManagerLogistica(logger, cnndrivin);
                        msg = mngLOG.List(estado, fechaini, fechafin);
                        break;
                    case 15: //Ruta 
                        ManagerRutas mngRUT = new ManagerRutas(logger, cnndrivin);
                        msg = mngRUT.List(estado);
                        break;
                    case 2010: //Picking
                        ManagerPicking mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.List(bodegacode, estado);
                        break;
                    case 3010: //Solicitud de produccion
                        ManagerProduccion mngPD = new ManagerProduccion(logger);
                        msg = mngPD.List(bodegacode, estado);
                        break;
                    case 16: //Solicitud materia prima
                        ManagerSolicitudMP mngMP = new ManagerSolicitudMP(logger);
                        msg = mngMP.List(bodegacode, estado);
                        break;
                    case 18: //Rendicion producccion
                        ManagerRendicion mngRend = new ManagerRendicion(logger);
                        msg = mngRend.List(estado);
                        break;
                }


                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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


        public MensajeReturn Historial(int docentry, int doctipo)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {
                ManagerPicking mngPK;
                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        ManagerNotadePedido mngNP = new ManagerNotadePedido(logger);
                        msg = mngNP.Historial(docentry);
                        break;
                }

                //**********************************

                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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
        public MensajeReturn Propiedades(int doctipo)
        {
            MensajeReturn msg = new MensajeReturn();
            try
            {

                switch (doctipo)
                {
                    case 10: //Nota de Pedido
                        break;
                    case 11: //Guia de despacho
                        break;
                    case 12: //Factura
                        break;
                    case 13: //Nota de Credito
                        break;
                    case 2010: //Picking
                        ManagerPicking mngPK = new ManagerPicking(logger, cnndf);
                        msg = mngPK.Propiedades();

                        break;
                }


                return msg;
            }
            catch (Exception ex)
            {
                //MensajeReturn msg = new MensajeReturn();
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
    }
}
