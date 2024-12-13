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
    public class ManagerAjustes
    {

        Logger logger;
        cnnDF cnndf;
        public ManagerAjustes(Logger _logger, cnnDF _cnndf)
        {
            logger = _logger;
            cnndf = _cnndf;
        }

        public MensajeReturn Add(Transaccion item)
        {
            MensajeReturn msg = null;
            try
            {
                //Repo_OITM repo = new Repo_OITM(logger);

                //grabar en db Colonos ------------
                Repo_OTRX repo = new Repo_OTRX();
                var json = repo.Add(item);
                var otrx = JsonConvert.DeserializeObject<Transaccion>(json);
                //---------------------------------

                if (otrx.DocEntry > 0)
                {
                    Repo_OBOD repoBo = new Repo_OBOD();
                    json = repoBo.List();
                    var listBodegas = JsonConvert.DeserializeObject<List<OBOD>>(json);

                    var trx = new TransacciondeInventario
                    {
                        folio = 0,
                        fiscalYear = item.DocFecha.Year.ToString(),
                        gloss = item.Observaciones,
                        originStowageId = item.BodegaCodeOrigen == "" ? "" : listBodegas.Find(x => x.BodegaCode == item.BodegaCodeOrigen).BodegaDF,
                        destinationStowageId = item.BodegaCodeDestino == "" ? "" : listBodegas.Find(x => x.BodegaCode == item.BodegaCodeDestino).BodegaDF,
                        total = 0,
                        date = item.DocFecha
                    };
                    trx.details = new List<DetailAjuste>();
                    foreach (var l in otrx.Lineas)
                    {
                        trx.details.Add(new DetailAjuste
                        {
                            analysis = new AnalisysAjuste(),
                            articleId = l.ProdCode,
                            coinId = "PESO",
                            comment = "",
                            count = l.CantidadSolicitada,
                            description = l.ProdNombre,
                            price = l.Costo
                        });
                    }
                    string metodo = "";
                    switch (item.TipoAjuste)
                    {
                        case 401: //egreso
                            trx.documentTypeId = "SALPROD";
                            trx.reasonId = "EGRESO";
                            metodo = "/api/inventory/Insert";
                            break;
                        case 402: //ingreso
                            trx.documentTypeId = "ENTPROD";
                            trx.reasonId = "INGRESO";
                            metodo = "/api/inventory/Insert";
                            break;
                        case 403: //trasaldo
                            trx.documentTypeId = "TRASPASO";
                            trx.reasonId = "TRASPASO";
                            metodo = "/api/inventory/InsertSkipCentralization";
                            break;
                    }

                    json = JsonConvert.SerializeObject(trx, Formatting.Indented);
                    AgenteDefontana df = new AgenteDefontana(logger, cnndf);
                    logger.Info("Ajuste inventario en DF: {0} \n {2}", trx.documentTypeId, json);

                    var token = df.RenovarToken(cnndf.metodoauth);
                    var result = "";
                    bool succes = false;
                    if (token != "")
                    {
                        result = df.ExecutePost(metodo, token, json, ref succes);
                    }
                    //----------------------------------
                    var docdf = JsonConvert.DeserializeObject<SaveSaleReturn>(result);
                    if (succes)
                    {
                        if (docdf.success)
                        {
                            //actualizar inventario en Colonos Web
                            Repo_OITB repoInv = new Repo_OITB();

                            foreach (var i in otrx.Lineas)
                            {
                                OITB stock = null;
                                if (otrx.BodegaCodeOrigen != "")
                                {
                                    json = repoInv.Get(i.ProdCode, otrx.BodegaCodeOrigen);
                                    stock = JsonConvert.DeserializeObject<OITB>(json);
                                    if (stock.Stock == null)
                                        stock.Stock = 0;
                                    stock.Stock += i.CantidadSolicitada * -1;
                                    repoInv.Modify(stock);
                                }

                                if (otrx.BodegaCodeDestino != "")
                                {
                                    json = repoInv.Get(i.ProdCode, otrx.BodegaCodeDestino);
                                    stock = JsonConvert.DeserializeObject<OITB>(json);
                                    if (stock.Stock == null)
                                        stock.Stock = 0;
                                    stock.Stock += i.CantidadSolicitada;
                                    repoInv.Modify(stock);
                                }
                            }
                            //actualizar estado OTRX
                            json = JsonConvert.SerializeObject(otrx);
                            var otrxdb = JsonConvert.DeserializeObject<OTRX>(json);
                            otrxdb.DocEstado = "C";
                            otrxdb.FolioDF = docdf.number;
                            repo.Modify(otrxdb);


                            msg = new MensajeReturn();
                            msg.statuscode = HttpStatusCode.OK;
                            msg.error = false;
                            msg.msg = "Ajuste Realizado";
                            msg.data = otrxdb;
                            return msg;
                        }
                        else
                        {
                            msg = new MensajeReturn();
                            msg.statuscode = HttpStatusCode.BadRequest;
                            msg.error = true;
                            msg.msg = docdf.message;
                            msg.data = docdf;
                            return msg;
                        }
                    }
                    else
                    {
                        msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.BadRequest;
                        msg.error = true;
                        msg.msg = docdf.message;
                        msg.data = docdf;
                        return msg;
                    }

                }
                else
                {
                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.InternalServerError;
                    msg.error = true;
                    msg.msg = "ajute no pudo ser generado en Colonos";
                    msg.data = "";
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
        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg = null;

            var json= JsonConvert.SerializeObject(doc);
            Transaccion item = JsonConvert.DeserializeObject<Transaccion>(json);
            msg = this.Add(item);
            return msg; 
        }
    }
}
