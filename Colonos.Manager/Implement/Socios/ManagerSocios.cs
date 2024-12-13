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
using System.Transactions;

namespace Colonos.Manager
{
    public class ManagerSocios
    {
        Logger logger;
        cnnDF cnndf;
        public ManagerSocios(Logger _logger, cnnDF _cnnDF)
        {
            logger = _logger;
            cnndf = _cnnDF;
        }

        public ManagerSocios(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string item)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var oscp = JsonConvert.DeserializeObject<OSCP>(item);
                var json = repo.Add(oscp);
                var list = JsonConvert.DeserializeObject<Socio>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Socio";
                msg.data = list;
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

        public MensajeReturn AddDF(string item)
        {
            try
            {
                Repo_JsonClientes repo = new Repo_JsonClientes();

                var oscp = JsonConvert.DeserializeObject<DataAccess.ClienteDF>(item);
                repo.Add(oscp);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Socio";
                msg.data = oscp.name;
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
        public MensajeReturn Get(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);
                Repo_SCP1 repodir = new Repo_SCP1(logger);
                Repo_SCP2 repocon=new Repo_SCP2(logger);
                Repo_SCP10 repoarch = new Repo_SCP10();
                Repo_SCP13 repo13 = new Repo_SCP13();

                var json = repo.Get(sociocode);
                var item = JsonConvert.DeserializeObject<Socio>(json);
                if (item != null)
                {
                    item.Direcciones = new List<Direccion>();
                    item.Contactos = new List<Contacto>();
                    json = repodir.List(sociocode);
                    item.Direcciones = JsonConvert.DeserializeObject<List<Direccion>>(json);
                    json = repocon.List(sociocode);
                    item.Contactos = JsonConvert.DeserializeObject<List<Contacto>>(json);
                    json = repoarch.List(sociocode);
                    item.Archivos = JsonConvert.DeserializeObject<List<Archivo>>(json);
                    json = repo13.List(sociocode);
                    item.Historial = JsonConvert.DeserializeObject<List<HistorialModificaciones>>(json);
                }
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = item == null ? true : false;
                msg.msg = item == null ? "Socio no existe" : "Socio";
                msg.data = item;
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
                Repo_OSCP repo = new Repo_OSCP(logger);
                ManagerDirecciones mng1 = new ManagerDirecciones(logger); 
                ManagerContactos mng2=new ManagerContactos(logger);
                ManagerArchivos mngA = new ManagerArchivos(logger);

                var socioinput = JsonConvert.DeserializeObject<Socio>(item);
                var json = JsonConvert.SerializeObject(socioinput.Direcciones);
                var dirupdate = JsonConvert.DeserializeObject<List<SCP1>>(json);
                json = JsonConvert.SerializeObject(socioinput.Contactos);
                var contacupdate = JsonConvert.DeserializeObject<List<SCP2>>(json);
                json = JsonConvert.SerializeObject(socioinput.Archivos);
                var archivosupdate= JsonConvert.DeserializeObject<List<SCP10>>(json);

                //recuperar datos actuales de direcciones y contactos -----
                var msgreturn = mng1.List(socioinput.SocioCode);
                var diractual = msgreturn.data;
                msgreturn = mng2.List(socioinput.SocioCode);
                var contactual = msgreturn.data;
                msgreturn = mngA.List(socioinput.SocioCode);
                var archactual = msgreturn.data;

                //actualizar datos ---------------------------------------
                var oscp = JsonConvert.DeserializeObject<OSCP>(item);
                json = repo.Get(oscp.SocioCode);
                var oscpactual = JsonConvert.DeserializeObject<OSCP>(json);
                //validar si ha cambiado algo
                string cambios = "";
                if (oscp.VendedorCode!=oscpactual.VendedorCode)
                {
                    cambios += String.Format("Vendedor: {0} ==> {1} <br>", oscpactual.VendedorCode, oscp.VendedorCode);
                }
                if (oscp.EstadoOperativo != oscpactual.EstadoOperativo)
                {
                    cambios += String.Format("Estado Operativo: {0} ==> {1} <br>", oscpactual.EstadoOperativo, oscp.EstadoOperativo);
                }
                if (oscp.CondicionDF != oscpactual.CondicionDF)
                {
                    cambios += String.Format("CondicionDF: {0} ==> {1} <br>", oscpactual.CondicionDF, oscp.CondicionDF);
                }
                if (oscp.CreditoAutorizado != oscpactual.CreditoAutorizado)
                {
                    cambios += String.Format("Credito Autorizado: {0} ==> {1} <br>", oscpactual.CreditoAutorizado, oscp.CreditoAutorizado);
                }
                if (oscp.CreditoMaximo != oscpactual.CreditoMaximo)
                {
                    cambios += String.Format("Credito Maximo: {0} ==> {1} <br>", oscpactual.CreditoMaximo, oscp.CreditoMaximo);
                }
                
                json = repo.Modify(oscp);
                if(cambios!="")
                {
                    //agregar registro en historial de cambios
                    Repo_SCP13 repo13 = new Repo_SCP13();
                    repo13.Add(new SCP13 { FechaProceso = DateTime.Now, RazonSocial = oscp.RazonSocial, Usuario = "", SocioCode = oscp.SocioCode, Cambios = cambios });
                }
                mng1.UpdateDirecciones(dirupdate, diractual);
                mng2.UpdateContactos(contacupdate, contactual);
                mngA.UpdateArchivos(archivosupdate, archactual);

                msgreturn = this.Get(oscp.SocioCode);
                var socio = msgreturn.data;

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = socio.SocioCode == null ? "Socio no existe" : "Actualizar Socio";
                msg.data = socio;
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

        public MensajeReturn Delete(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);
                Repo_SCP1 repodr = new Repo_SCP1(logger);
                Repo_SCP2 repocn = new Repo_SCP2(logger);
                using (TransactionScope transaction = new TransactionScope())
                {
                    var item = repodr.Delete(sociocode);
                    item = repocn.Delete(sociocode);
                    item = repo.Delete(sociocode);
                    transaction.Complete();
                    MensajeReturn msg = new MensajeReturn();
                    msg.statuscode = item ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                    msg.error = item;
                    msg.msg = !item ? "Socio no existe" : "Socio";
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
        public MensajeReturn List()
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<Socio>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Socios";
                msg.data = list;
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

        public MensajeReturn List(string palabras, string tipo, string usuario)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.List(palabras, tipo, usuario);
                var list = JsonConvert.DeserializeObject<List<SociosResult>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado Socios";
                msg.data = list;
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

        public MensajeReturn Ventas12Meses(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.Ventas12Meses(sociocode);
                var list = JsonConvert.DeserializeObject<List<VentaSocio>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado Ventas 12 meses";
                msg.data = list;
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

        public MensajeReturn Propiedades()
        {
            try
            {

                Repo_SCP3 repo3 = new Repo_SCP3();
                Repo_SCP12 repo12 = new Repo_SCP12();
                Repo_SCP5 repo5 = new Repo_SCP5();
                Repo_SCP6 repo6 = new Repo_SCP6();
                Repo_SCP7 repo7 = new Repo_SCP7();
                Repo_SCP8 repo8 = new Repo_SCP8();
                Repo_SCP9 repo9 = new Repo_SCP9();

                

                Repo_OCOM repoco = new Repo_OCOM();
                Repo_OCIU repoci = new Repo_OCIU();
                Repo_OREG reporg = new Repo_OREG();
                Repo_OVEN repovd = new Repo_OVEN();
                Repo_ZON1 repozon1 = new Repo_ZON1();

                var prop = new SocioPropiedades();

                //var json = repo3.List(); prop.condicion = JsonConvert.DeserializeObject<List<SCP3>>(json);
                var json = repo12.List(); prop.condicion = JsonConvert.DeserializeObject<List<SCP12>>(json);
                json = repo5.List(); prop.rubro = JsonConvert.DeserializeObject<List<SCP5>>(json);
                json = repo6.List(); prop.tiposocio = JsonConvert.DeserializeObject<List<SCP6>>(json);
                json = repo7.List(); prop.estadosoperativo = JsonConvert.DeserializeObject<List<SCP7>>(json);
                json = repo9.List(); prop.matriz = JsonConvert.DeserializeObject<List<SCP9>>(json);
                json = repoco.List(); prop.comunas = JsonConvert.DeserializeObject<List<OCOM>>(json);
                json = repoci.List(); prop.ciudades = JsonConvert.DeserializeObject<List<OCIU>>(json);
                json = reporg.List(); prop.regiones = JsonConvert.DeserializeObject<List<OREG>>(json);
                json = repovd.List(); prop.vendedores = JsonConvert.DeserializeObject<List<OVEN>>(json);
                json = repo8.List(); prop.tipocontacto = JsonConvert.DeserializeObject<List<SCP8>>(json);
                json = repozon1.List(); prop.subzona = JsonConvert.DeserializeObject<List<ZON1>>(json);
                

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = prop.condicion.Count +
                    prop.rubro.Count +
                    prop.tiposocio.Count +
                    prop.estadosoperativo.Count +
                    prop.matriz.Count +
                    prop.comunas.Count +
                    prop.ciudades.Count +
                    prop.vendedores.Count;
                msg.msg = "Propiedades Socio";
                msg.data = prop;
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

        public MensajeReturn TopFamiliaCliente(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.TopFamiliaCliente(sociocode);
                var list = JsonConvert.DeserializeObject<List<spTopCliente_Result>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Top Cliente";
                msg.data = list;
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

        public MensajeReturn TopVentasCliente(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.TopVentasCliente(sociocode);
                var list = JsonConvert.DeserializeObject<List<spUltimasVentasCliente_Result>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Top Cliente";
                msg.data = list;
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
        public MensajeReturn TopFamiliaRubro(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.TopFamiliaRubro(sociocode);
                var list = JsonConvert.DeserializeObject<List<spTopRubro_Result>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Top Rubro";
                msg.data = list;
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

        public MensajeReturn TopUltimosPrecios(string sociocode, string familiacode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);

                var json = repo.TopUltimosPrecios(sociocode, familiacode);
                var list = JsonConvert.DeserializeObject<List<spUltimosPreciosClienteFamilia_Result>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Top Rubro";
                msg.data = list;
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

        public MensajeReturn CuentaCorriente(string sociocode)
        {
            try
            {
                Repo_OSCP repo = new Repo_OSCP(logger);
                Repo_OPED repoventa = new Repo_OPED(logger);
                 
                var json = repo.CuentaCorriente(sociocode);
                var list = JsonConvert.DeserializeObject<List<SCP11>>(json);
                json = repoventa.ListVentas(sociocode);
                var listped = JsonConvert.DeserializeObject<List<OPED>>(json);
                foreach(var p in listped)
                {
                    if (p.Facturado == null)
                        p.Facturado = 0;

                    SCP11 item = new SCP11 { 
                        SocioCode=p.SocioCode,
                        SocioNombre=p.RazonSocial,
                        Numero =p.DocEntry,
                        Vencimiento=String.Format("{0:dd/MM/yyyy}",p.DocFecha),
                        V91=0,
                        V90=0,
                        V60=0,
                        V30 =p.Total - p.Facturado
                    };
                    list.Add(item);
                }
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list != null ? list.Count() : 0;
                msg.msg = "Cuenta Corriente";
                msg.data = list;
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

        public MensajeReturn GetClienteDefontana(string sociocode)
        {
            try
            {
                AgenteDefontana df = new AgenteDefontana(logger, cnndf);
             
                var token = df.RenovarToken(cnndf.metodoauth);
                var result = "";
                bool succes = false;
                var json = "";

                Socio socio = null;
                //Socio socio = new Socio {
                //    Contactos = new List<Contacto>(),
                //    Direcciones = new List<Direccion>()
                //};

                if (token!=null && token != "")
                {
                    var url =String.Format("{0}{1}{2}",cnndf.metodoclientes, "?itemsPerPage=250&pageNumber=1&status=0&fileID=",sociocode);
                    result = df.ExecuteGet(url, token,json, ref succes);
                    var resultDF = JsonConvert.DeserializeObject<GetResultDF>(result);
                    if(resultDF.success && resultDF.clientList!=null && resultDF.clientList.Count>0)
                    {
                        var cliDF = resultDF.clientList[0];
                        if (cliDF.sellerID != null && cliDF.paymentID != null)
                        {


                            var socioCode = String.Format("{0}C", cliDF.legalCode.Replace(".", "").Trim());
                            var returnget = this.Get(socioCode);
                            if (!returnget.error) //el cliente existe en colonos
                            {
                                Socio cliCol = returnget.data; // JsonConvert.DeserializeObject<Socio>(returnget.data);
                                if (cliCol != null && cliCol.SocioCode != null)
                                {

                                    cliCol.VendedorCode = cliDF.sellerID;
                                    cliCol.clientFileDF = cliDF.legalCode;
                                    cliCol.CondicionDF = cliDF.paymentID;
                                    this.Modify(JsonConvert.SerializeObject(cliCol));
                                }
                            }
                            else
                            {
                                //crear cliente en colonos
                                socioCode = String.Format("{0}{1}", cliDF.legalCode.Replace(".", "").Trim(), "C");
                                Socio cliCol = new Socio
                                {
                                    Contactos = new List<Contacto>(),
                                    Direcciones = new List<Direccion>(),
                                    SocioCode = socioCode,
                                    RazonSocial = cliDF.name,
                                    NombreFantasia = cliDF.fantasyname,
                                    clientFileDF = cliDF.fileID,
                                    CondicionDF = cliDF.paymentID,
                                    VendedorCode = cliDF.sellerID,
                                    CreditoAutorizado = 0,
                                    CreditoMaximo = 0,
                                    CreditoUtiliado = 0,
                                    Giro = cliDF.business,
                                    SocioTipo = "C",
                                    EstadoOperativo = "ACT",
                                    Rut = cliDF.legalCode.Replace(".", "").Trim(),
                                };

                                Direccion dir = new Direccion
                                {
                                    Calle = cliDF.address,
                                    CiudadNombre = cliDF.city,
                                    ComunaNombre = cliDF.district,
                                    DireccionTipo = "F",
                                    SocioCode = socioCode
                                };

                                this.Add(JsonConvert.SerializeObject(cliCol));

                                ManagerDirecciones mngdir = new ManagerDirecciones(logger);

                                mngdir.Add(JsonConvert.SerializeObject(dir));


                            }
                            //validar si existe en Colonos web
                            //de no existir,crear cliente en Colonos web
                            //si existe , actualizar cliente en Colonos web

                            var m = this.Get(socioCode);
                            if (!m.error)
                            {

                                //retornar sociocode
                                socio = m.data;
                            }
                        }
                    }

                }
                var list = JsonConvert.DeserializeObject<List<SCP11>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = 0;
                msg.msg = "Socio";
                msg.data = socio;
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
        
    }
}
