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
    public class ManagerRecetas
    {
        Logger logger;
        public ManagerRecetas(Logger _logger)
        {
            logger = _logger;
        }

        public MensajeReturn Add(string jsonentrada)
        {
            try
            {

                var receta = JsonConvert.DeserializeObject<RecetaProducto>(jsonentrada);

                var item=JsonConvert.SerializeObject(receta);
                var lineas = JsonConvert.SerializeObject(receta.Lineas);
                //encabesado ------
                Repo_OITR repo = new Repo_OITR(logger);
                var oitr = JsonConvert.DeserializeObject<OITR>(item);
                var json = repo.Add(oitr);

                //detalle ----
                Repo_ITR1 repod = new Repo_ITR1();
                var itr1 = JsonConvert.DeserializeObject<List<ITR1>>(lineas);
                foreach(var l in itr1)
                {
                    repod.Add(l);
                }

                Repo_OITM repoprod = new Repo_OITM(logger);
                json = repoprod.Get(receta.ProdCode);
                var prod = JsonConvert.DeserializeObject<OITM>(json);
                prod.TieneReceta = "S";
                repoprod.Modify(prod);
                //Retornar datos creados

                receta= JsonConvert.DeserializeObject<RecetaProducto>(repo.Get(oitr.ProdCode));
                receta.Lineas = JsonConvert.DeserializeObject<List<RecetaProductoList>>(repod.List(oitr.ProdCode));
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Receta";
                msg.data = receta;
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

        public MensajeReturn Get(string prodcode, string bodegacode)
        {
            try
            {
                Repo_OITR repo = new Repo_OITR(logger);
                Repo_OITM repoprod = new Repo_OITM(logger);
                Repo_OITB repostock = new Repo_OITB();

                var json = repo.Get(prodcode);
                var item = JsonConvert.DeserializeObject<RecetaProducto>(json);
                if(item==null)
                {
                    MensajeReturn msg1 = new MensajeReturn();
                    msg1.statuscode = HttpStatusCode.BadRequest;
                    msg1.error = true;
                    msg1.msg = "Receta no existe";
                    msg1.data = null;

                    return msg1;
                }
                Repo_ITR1 repod = new Repo_ITR1();
                json = repod.List(prodcode, bodegacode);
                var list = JsonConvert.DeserializeObject<List<RecetaProductoList>>(json);
                item.Lineas = new List<RecetaProductoList>();
                foreach(var l in list)
                {
                    item.Lineas.Add(l);
                }
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = item == null ? "Receta no existe" : "Receta";
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

        public MensajeReturn Modify(string jsonentrada)
        {
            try
            {
                var receta = JsonConvert.DeserializeObject<RecetaProducto>(jsonentrada);

                var item = JsonConvert.SerializeObject(receta);
                var lineas = JsonConvert.SerializeObject(receta.Lineas);
            
                //encabezado ------------
                Repo_OITR repo = new Repo_OITR(logger);
                var oitm = JsonConvert.DeserializeObject<OITR>(item);
                var json = repo.Modify(oitm);

                //detalle ---------------
                Repo_ITR1 repod = new Repo_ITR1();
                var linoriginal = JsonConvert.DeserializeObject<List<RecetaProductoList>>(repod.List(receta.ProdCode));
                var linupdate = receta.Lineas;

                UpdateDetalle(linupdate, linoriginal);

                var producto = JsonConvert.DeserializeObject<Producto>(json);
                json = repo.Get(oitm.ProdCode);
                receta = JsonConvert.DeserializeObject<RecetaProducto>(json);
                json = repod.List(oitm.ProdCode);
                receta.Lineas = JsonConvert.DeserializeObject<List<RecetaProductoList>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = producto.ProdCode == null ? "Receta no existe" : "Actualizar Receta";
                msg.data = receta;
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

        private void UpdateDetalle(List<RecetaProductoList> ItemsUpdate, List<RecetaProductoList> ItemsCurr)
        {
            Repo_ITR1 repo = new Repo_ITR1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (RecetaProductoList ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<ITR1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<ITR1>(json);
                        repo.Delete(lin.ProdCode,lin.ProdCodeRef);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<RecetaProductoList> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        RecetaProductoList cd = ItemsUpdate.Find(x => x.ProdCode == i.ProdCode && i.ProdCodeRef== x.ProdCodeRef);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<ITR1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<ITR1>(json);
                            repo.Delete(lin.ProdCode, lin.ProdCodeRef);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<ITR1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }


        public MensajeReturn Delete(string prodcode)
        {
            try
            {
                Repo_OITR repo = new Repo_OITR(logger);
                Repo_ITR1 repod = new Repo_ITR1();

                var json = repod.List(prodcode);
                var list = JsonConvert.DeserializeObject<List<RecetaProductoList>>(json);
                foreach (var l in list)
                {
                    repod.Delete(l.ProdCode, l.ProdCodeRef);
                }

                var item = repo.Delete(prodcode);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = item ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = item;
                msg.msg = !item ? "Receta no existe" : "Receta";
                msg.data = "";
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

        public MensajeReturn List()
        {
            try
            {
                Repo_OITR repo = new Repo_OITR(logger);

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<RecetaProducto>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Receta";
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

        public MensajeReturn List(string palabras, string sku)
        {
            try
            {
                Repo_OITR repo = new Repo_OITR(logger);

                var json = repo.List(palabras, sku);
                var list = JsonConvert.DeserializeObject<List<ProductosResult>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado Receta";
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
    }
}
