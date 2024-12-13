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
    public class ManagerPreciosFijos
    {
        Logger logger;
        public ManagerPreciosFijos(Logger _logger)
        {
            logger = _logger;
        }
        public MensajeReturn Add(string jsonentrada)
        {
            try
            {

                var receta = JsonConvert.DeserializeObject<ListaPrecioFijo>(jsonentrada);

                var item = JsonConvert.SerializeObject(receta);
                var lineas = JsonConvert.SerializeObject(receta.Lineas);
                //encabesado ------
                Repo_OLPC repo = new Repo_OLPC();
                var olpc = JsonConvert.DeserializeObject<OLPC>(item);
                repo.Add(olpc);

                //detalle ----
                Repo_LPC1 repod = new Repo_LPC1();
                var lpc1 = JsonConvert.DeserializeObject<List<LPC1>>(lineas);
                foreach (var l in lpc1)
                {
                    repod.Add(l);
                }

                //Retornar datos creados

                receta = JsonConvert.DeserializeObject<ListaPrecioFijo>(repo.Get(olpc.ListaCode));
                receta.Lineas = JsonConvert.DeserializeObject<List<ListaPrecioFijoList>>(repod.List(olpc.ListaCode));
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = "Precios Fijos";
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

        public MensajeReturn Get(string listacode)
        {
            try
            {
                Repo_OLPC repo = new Repo_OLPC();
                Repo_OITM repoprod = new Repo_OITM(logger);
                //Repo_OITB repostock = new Repo_OITB();

                var json = repo.Get(listacode);
                var item = JsonConvert.DeserializeObject<ListaPrecioFijo>(json);
                if (item == null)
                {
                    MensajeReturn msg1 = new MensajeReturn();
                    msg1.statuscode = HttpStatusCode.BadRequest;
                    msg1.error = true;
                    msg1.msg = "Lista de precio fijo no existe";
                    msg1.data = null;

                    return msg1;
                }
                Repo_LPC1 repod = new Repo_LPC1();
                json = repod.List(listacode);
                var list = JsonConvert.DeserializeObject<List<ListaPrecioFijoList>>(json);
                item.Lineas = new List<ListaPrecioFijoList>();
                foreach (var l in list)
                {
                    l.Margen = l.Precio == 0 ? 0 : (l.Precio - l.Costo) / l.Precio;
                    item.Lineas.Add(l);
                }
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = item == null ? "Lista de precio fijo no existe" : "Lista de precio fijo";
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
                var listap = JsonConvert.DeserializeObject<ListaPrecioFijo>(jsonentrada);

                var item = JsonConvert.SerializeObject(listap);
                var lineas = JsonConvert.SerializeObject(listap.Lineas);

                //encabezado ------------
                Repo_OLPC repo = new Repo_OLPC();
                var olpc = JsonConvert.DeserializeObject<OLPC>(item);
                var json = repo.Modify(olpc);

                //detalle ---------------
                Repo_LCP1 repod = new Repo_LCP1();
                var linoriginal = JsonConvert.DeserializeObject<List<ListaPrecioFijoList>>(repod.List(listap.ListaCode));
                var linupdate = listap.Lineas;

                UpdateDetalle(linupdate, linoriginal);

                //var producto = JsonConvert.DeserializeObject<Producto>(json);
                json = repo.Get(olpc.ListaCode);
                listap = JsonConvert.DeserializeObject<ListaPrecioFijo>(json);
                json = repod.List(olpc.ListaCode);
                listap.Lineas = JsonConvert.DeserializeObject<List<ListaPrecioFijoList>>(json);

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = listap == null ? "Lista de precio fijo no existe" : "Lista de precio fijo";
                msg.data = listap;
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

        private void UpdateDetalle(List<ListaPrecioFijoList> ItemsUpdate, List<ListaPrecioFijoList> ItemsCurr)
        {
            Repo_LPC1 repo = new Repo_LPC1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (ListaPrecioFijoList ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<LPC1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<LPC1>(json);
                        repo.Delete(lin.ProdCode, lin.ProdCode);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<ListaPrecioFijoList> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        ListaPrecioFijoList cd = ItemsUpdate.Find(x => x.ProdCode == i.ProdCode && i.ListaCode == x.ListaCode);
                        if (cd != null)
                        {
                            json = JsonConvert.SerializeObject(cd);
                            var lin = JsonConvert.DeserializeObject<LPC1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<LPC1>(json);
                            repo.Delete(lin.ProdCode, lin.ListaCode);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<LPC1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }


        public MensajeReturn Delete(string listacode)
        {
            try
            {
                Repo_OLPC repo = new Repo_OLPC();
                Repo_LPC1 repod = new Repo_LPC1();

                var json = repod.List(listacode);
                var list = JsonConvert.DeserializeObject<List<ListaPrecioFijoList>>(json);
                foreach (var l in list)
                {
                    repod.Delete(l.ProdCode, l.ListaCode);
                }

                repo.Delete(listacode);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = true ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = false;
                msg.msg = "Lista de precio fijo";
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


        public MensajeReturn Delete(string listacode, string prodcode)
        {
            try
            {
                Repo_OLPC repo = new Repo_OLPC();
                Repo_LPC1 repod = new Repo_LPC1();

                var json = repod.List(listacode);
                var list = JsonConvert.DeserializeObject<List<ListaPrecioFijoList>>(json);
                foreach (var l in list)
                {
                    repod.Delete(l.ProdCode, l.ListaCode);
                }

                repo.Delete(listacode);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = true ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = false;
                msg.msg = "Lista de precio fijo";
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
                Repo_OLPC repo = new Repo_OLPC();

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<ListaPrecioFijo>>(json);
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
                var list = JsonConvert.DeserializeObject<List<ListaPrecioFijo>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado LC";
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
