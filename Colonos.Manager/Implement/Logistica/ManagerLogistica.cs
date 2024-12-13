using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.DrivIn;
using Colonos.Entidades;
using Colonos.Entidades.Defontana;
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
    public class ManagerLogistica
    {
        Logger logger;
        cnnDrivin cnndrivin;
        public ManagerLogistica(Logger _logger, cnnDrivin _cnndrivin)
        {
            logger = _logger;
            cnndrivin = _cnndrivin;
        }

        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {
                

                Repo_OLOG repo = new Repo_OLOG(logger);
                var json = repo.Add(doc);
                var docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Ruta";
                msg.data = docgenerado;
                return msg;
            }
            catch(Exception ex)
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

        public MensajeReturn ModifySets(string item)
        {
            MensajeReturn msg = new MensajeReturn();

            var docs = JsonConvert.DeserializeObject<List<Documento>>(item);
            bool crearCustorio = false;
            string json = "";

            foreach (var doc in docs)
            {
                Documento ocus = null;
                if (doc.Custodio!=null && doc.TipoCustodio.Length>0 && !crearCustorio)
                {
                    crearCustorio = true;
                    ocus = new Documento();
                    ocus.DocTipo = doc.TipoCustodio == "RCT" ? 4015 : 4016; // RCT=4015, RCC=4016
                    ocus.DocEstado = "A";
                    ocus.DocFecha = DateTime.Now.Date;
                    ocus.Custodio = doc.Custodio;
                    ocus.TipoCustodio = doc.TipoCustodio;
                    ocus.ObservacionesCierre = doc.ObservacionesCierre;
                    ocus.EstadoOperativo = "ING";
                    ocus.BaseEntry = doc.BaseRuta;
                    ocus.BaseTipo = 15;
                    ocus.FechaRegistro = DateTime.Now;
                    ocus.UsuarioCode = doc.UsuarioCode;
                    ocus.Version = doc.Version;
                    ocus.UsuarioNombre = doc.UsuarioNombre;

                    Repo_OCUS repocus = new Repo_OCUS();
                    json=repocus.Add(ocus);
                    ocus = JsonConvert.DeserializeObject<Documento>(json);
                }
                if(ocus!=null && ocus.DocEntry>0)
                {
                    doc.BaseEntryCustodio = ocus.DocEntry;
                    doc.BaseTipoCustodio = ocus.DocTipo;
                }

                json = JsonConvert.SerializeObject(doc);
                msg=this.Modify(json);
            }


            return msg;
        }
        public MensajeReturn Modify(string item)
        {

            try
            {
                Documento doc = JsonConvert.DeserializeObject<Documento>(item);

                Repo_OLOG repo = new Repo_OLOG(logger);
                Repo_LOG1 repolin = new Repo_LOG1();
                var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var linupdate = doc.Lineas;

                UpdateDetalle(linupdate, linoriginal);
                var olog = JsonConvert.DeserializeObject<OLOG>(item);

                if (olog.Custodio != null && olog.Custodio.Length > 0)
                {
                    //generar RCT o RCC ??

                }
                var json = repo.Modify(olog);
                var ped = JsonConvert.DeserializeObject<Documento>(json);
                ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));

                if (olog.DocEstado == "C")
                {
                    Repo_OFAC repofac = new Repo_OFAC(logger);
                    Repo_FAC1 repofac1 = new Repo_FAC1(logger);

                    json = repofac.Get(Convert.ToInt32(olog.BaseEntry));
                    var fac = JsonConvert.DeserializeObject<Documento>(json);
                    var ofac = JsonConvert.DeserializeObject<OFAC>(json);
                    if (olog.RutaExitosa == "SI")
                    {
                        ofac.DocEstado = "C";
                        foreach (var d in fac.Lineas)
                        {
                            json = repofac1.Get(d.DocLinea);
                            var fac1 = JsonConvert.DeserializeObject<FAC1>(json);
                            fac1.LineaEstado = "C";
                            repofac1.Modify(fac1);
                        }
                    }
                    else
                    {
                        ofac.EstadoOperativo = "ING";
                    }

                    repofac.Modify(ofac);

                }
                //validar si todas las olog de la ruta estan cerradas para cerrar ORUT
                json = repo.List(olog.scenario_token);
                var listolog = JsonConvert.DeserializeObject<List<OLOG>>(json);
                bool cerrar = true;
                foreach (var d in listolog)
                {
                    if (d.DocEstado == "A")
                    {
                        cerrar = false;
                    }
                }
                if (cerrar)
                {
                    Repo_ORUT reporut = new Repo_ORUT(logger);
                    json = reporut.Get(olog.scenario_token);
                    var orut = JsonConvert.DeserializeObject<ORUT>(json);
                    orut.DocEstado = "C";
                    reporut.Modify(orut);

                }


                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Factura no existe" : "Actualizar Factura";
                msg.data = ped;

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

        public MensajeReturn Get(int docentry)
        {
            Repo_OLOG repo = new Repo_OLOG(logger);
            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Ruta no existe" : "Ruta";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn Search(string palabras, string vendedocode)
        {
            Repo_OLOG repo = new Repo_OLOG(logger);

            var json = repo.Search(palabras, vendedocode);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Rutas";
            msg.data = list;
            return msg;
        }

        public MensajeReturn List()
        {
            Repo_OLOG repo = new Repo_OLOG(logger);
            var json = repo.List();
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Rutas";
            msg.data = list;
            return msg;
        }
        public MensajeReturn List(string estado, string fechaini, string fechafin)
        {
            Repo_OLOG repo = new Repo_OLOG(logger);
            var json = repo.List(estado, fechaini, fechafin);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Rutas";
            msg.data = list;
            return msg;
        }

        public MensajeReturn ListVehiculos()
        {
            MensajeReturn msg = null;
            AgenteDrivin drivin = new AgenteDrivin(logger, cnndrivin);
            bool succes = false;
            var result = drivin.ExecuteGet(cnndrivin.metodovehicles, cnndrivin.XAPIKey, "", ref succes);
            var status= JsonConvert.DeserializeObject<DrivinBaseResult>(result);
            
            if(status.status!="error")
            {
                var drivinresult = JsonConvert.DeserializeObject<DrivinResult>(result);
                var list = JsonConvert.DeserializeObject<VehicleResult>(result);

                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = 1;
                msg.msg = "Vehiculos Drivin";
                list.response.Add(new Vehicle { id = "", code = "" });
                msg.data = list == null ? new List<Vehicle>() : list.response;

                return msg;
            }
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = true;
            msg.count = 1;
            msg.msg = result.ToString();
            msg.data = "";

            return msg;


        }
        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_LOG1 repo = new Repo_LOG1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<LOG1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<LOG1>(json);
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
                            var lin = JsonConvert.DeserializeObject<LOG1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<LOG1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<LOG1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }
    }
}
