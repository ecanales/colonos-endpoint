using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.DrivIn;
using Colonos.Entidades;
using Colonos.Entidades.Drivin;
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
    public class ManagerRutas
    {
        Logger logger;
        cnnDrivin cnndrivin;
        public ManagerRutas(Logger _logger, cnnDrivin _cnndrivin)
        {
            logger = _logger;
            cnndrivin = _cnndrivin;
        }
        public MensajeReturn Add(DocumentoRuta docruta)
        {
            MensajeReturn msg;
            try
            {

                //primero grabar en Drivin ------
                AgenteDrivin drivin = new AgenteDrivin(logger, cnndrivin);
                //-generar Escenario
                EscenarioRequest sce = new EscenarioRequest
                {
                    description = docruta.descripcion,
                    date = String.Format("{0:yyyy-MM-dd}", docruta.DocFecha),
                    schema_name = cnndrivin.schema_name,
                };
                bool succes = false;
                var json = JsonConvert.SerializeObject(sce, Formatting.Indented);
                var result = drivin.ExecutePost(cnndrivin.metodoscenarios, cnndrivin.XAPIKey, json, ref succes);
                var scedriv = JsonConvert.DeserializeObject<ScenarioReturn>(result);
                if (succes)
                {
                    docruta.scenario_token = scedriv.response.scenario_token;
                    docruta.schema_name = sce.schema_name;
                    Repo_ORUT reporut = new Repo_ORUT(logger);
                    json = reporut.Add(docruta);
                    var docrutagenerado = JsonConvert.DeserializeObject<DocumentoRuta>(json);

                    Repo_OSCP repocli = new Repo_OSCP(logger);
                    Repo_SCP1 repodir = new Repo_SCP1(logger);
                    if (scedriv.success)
                    {
                        docruta.scenario_token = scedriv.response.scenario_token;
                    }
                    //-crear rutas y asociarlas al escenario
                    RutaRequest ruta = new RutaRequest();
                    ruta.vehicle_code = docruta.Vehiculo;
                    
                    ruta.clients = new List<ClienteRequest>();

                    foreach (var doc in docruta.clients)
                    {
                        json = repocli.Get(doc.SocioCode);
                        var cli = JsonConvert.DeserializeObject<OSCP>(json);
                        json = repodir.Get(Convert.ToInt32(doc.DireccionCode));
                        SCP1 dir = null;
                        if (Convert.ToBoolean(doc.RetiraCliente))
                        {

                        }
                        else
                        {
                            dir = JsonConvert.DeserializeObject<SCP1>(json);
                        }
                        var cliente = new ClienteRequest
                        {

                            code = cli.Rut,
                            client_name = cli.RazonSocial,
                            address = String.Format("{0} {1}", dir.Calle, dir.Numero),
                            reference = dir.Observaciones,
                            city = dir.CiudadNombre,
                            county = dir.ComunaNombre,
                            country = "Chile",
                            contact_email = dir.EmailDriveIn,
                            contact_name = doc.RazonSocial.ToString(), // .ContactoCode.ToString(),
                            lat = Convert.ToDecimal(dir.Latitud),
                            lng = Convert.ToDecimal(dir.Longitud),
                            time_windows = new List<Time_windows>(),
                            orders = new List<OrderRequest>(),
                            name= cli.RazonSocial,
                        };
                        cliente.time_windows.Add(new Time_windows
                        {
                            start = dir.Ventana_Inicio,
                            end = dir.Ventana_Termino
                        });

                        var order = new OrderRequest
                        {
                            code = doc.FolioDF.ToString(),
                            description = doc.RazonSocial,
                            supplier_name = String.Format("{0} | {1} | {2}", "LCDN", doc.CondicionNombre, doc.RazonSocial),
                            items = new List<ItemRequest>()
                        };

                        foreach (var r in doc.Lineas)
                        {
                            ItemRequest item = new ItemRequest
                            {
                                code = r.ProdCode,
                                description = r.ProdNombre,
                                units_1 = Convert.ToDecimal(r.CantidadEntregada)
                            };
                            order.items.Add(item);
                        }
                        cliente.orders.Add(order);
                        ruta.clients.Add(cliente);
                        //-------------------------------
                    }

                    //-enviar a drivin ---
                    succes = false;
                    json = JsonConvert.SerializeObject(ruta, Formatting.Indented);
                    string url = String.Format("{0}?token={1}", cnndrivin.metodorutas, docruta.scenario_token);
                    result = drivin.ExecutePost(url, cnndrivin.XAPIKey, json, ref succes);
                    var rutadriv = JsonConvert.DeserializeObject<ScenarioReturn>(result);
                    if (succes)
                    {
                        
                        //-actualizar doc
                        foreach (var doc in docruta.clients)
                        {
                            doc.scenario_token = docruta.scenario_token;
                            doc.Vehiculo = docruta.Vehiculo;
                        }

                        //-crear OLOG
                        ManagerLogistica mng = new ManagerLogistica(logger, cnndrivin);
                        Repo_OFAC repo = new Repo_OFAC(logger);
                        bool hayerror = false;
                        string erromsg = "";
                        foreach (var doc in docruta.clients)
                        {
                            var fac = JsonConvert.DeserializeObject<OFAC>(JsonConvert.SerializeObject(doc));
                            var detfac = JsonConvert.DeserializeObject<List<FAC1>>(JsonConvert.SerializeObject(doc.Lineas));
                            var log = JsonConvert.DeserializeObject<OLOG>(JsonConvert.SerializeObject(doc));
                            var detlog = JsonConvert.DeserializeObject<List<LOG1>>(JsonConvert.SerializeObject(doc.Lineas));

                            log.BaseEntry = fac.DocEntry;
                            log.BaseTipo = fac.DocTipo;
                            log.DocTipo = 14;
                            log.DocFecha = DateTime.Now.Date;
                            log.FechaRegistro = DateTime.Now;
                            log.BaseRuta = docruta.DocEntry;
                            log.clientFileDF = fac.clientFileDF;
                            log.FolioDF = fac.FolioDF;
                            log.DireccionCodeFact = fac.DireccionCode;
                            log.CondicionDF = fac.CondicionDF;
                            log.BasePedido = fac.BaseEntry;

                            foreach (var de in detfac)
                            {
                                detlog.Find(x => x.DocLinea == de.DocLinea).BaseLinea = de.DocLinea;
                                detlog.Find(x => x.DocLinea == de.DocLinea).BaseEntry = de.DocEntry;
                                detlog.Find(x => x.DocLinea == de.DocLinea).BaseTipo = de.DocTipo;
                            }

                            var docLog = JsonConvert.DeserializeObject<Documento>(JsonConvert.SerializeObject(log));
                            docLog.Lineas= JsonConvert.DeserializeObject<List<DocumentoLinea>>(JsonConvert.SerializeObject(detlog));

                            MensajeReturn msg2 = mng.Add(docLog);
                            if (!msg2.error)
                            {
                                fac.EstadoOperativo = "RUTA";
                                fac.BaseEntryLog = docLog.DocEntry;
                                repo.Modify(fac);
                            }
                            else
                            {
                                hayerror = true;
                                erromsg += String.Format("{0} \n", msg2.msg);
                            }
                        }
                        if (!hayerror)
                        {
                            msg = new MensajeReturn();
                            msg.statuscode = HttpStatusCode.OK;
                            msg.count = docruta.clients.Count;
                            msg.error = false;
                            msg.msg = String.Format("Ruta Generada: {0}", rutadriv.response.scenario_token);
                            msg.data = docrutagenerado;
                            return msg;
                        }
                        else
                        {
                            msg = new MensajeReturn();
                            msg.statuscode = HttpStatusCode.Conflict;
                            msg.count = 0;
                            msg.error = false;
                            msg.msg = String.Format("Algunos documentos OLOG no pudieron ser creados en Colonos: {0}", erromsg);
                            msg.data = null;
                            logger.Error("{0}", String.Format("Algunos documentos OLOG no pudieron ser creados en Colonos: {0}", erromsg));
                            return msg;
                        }
                    }
                    else
                    {
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.InternalServerError;
                        msg.count = 0;
                        msg.error = true;
                        msg.msg = String.Format("Ruta no pudo ser generada en ORUT DriveIn: {0}", rutadriv.response.description);
                        msg.data = null;
                        logger.Error("{0}", String.Format("Ruta no pudo ser generada en ORUT DriveIn: {0}", rutadriv.response.description));
                        return msg;
                    }
                }
                else
                {
                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.InternalServerError;
                    msg.count = 0;
                    msg.error = true;
                    msg.msg = String.Format("Escenario no pudo ser generado en DriveIn: {0}", scedriv.response.description);
                    msg.data = null;
                    logger.Error("{0}", String.Format("Escenario no pudo ser generado en DriveIn: {0}", scedriv.response.description));
                    return msg;
                }
            }
            catch(Exception ex)
            {
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.InternalServerError;
                msg.count = 0;
                msg.error = true;
                msg.msg = String.Format("Ruta no pudo ser generada en ORUT DriveIn: {0}", ex.Message);
                msg.data = null;
                logger.Error("{0}",String.Format("Ruta no pudo ser generada en ORUT DriveIn: {0}", ex.Message));
                logger.Error("{0}", ex.StackTrace);
                return msg;
            }
        }

        public MensajeReturn List(string estado)
        {
            Repo_ORUT repo = new Repo_ORUT(logger);
            var json = repo.List(estado);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Rutas";
            msg.data = list;
            return msg;
        }
    }
}
