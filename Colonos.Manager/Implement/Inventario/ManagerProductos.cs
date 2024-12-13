using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.Defontana;
using Colonos.Entidades;
using Colonos.Entidades.Defontana;
using Newtonsoft.Json;
using NLog;

namespace Colonos.Manager
{
    public class ManagerProductos
    {
        Logger logger;
        cnnDF cnndf;
        public ManagerProductos(Logger _logger, cnnDF _cnndf)
        {
            logger = _logger;
            cnndf = _cnndf;
        }
        //public List<Producto> ListarProductos()
        //{
        //    Repo_OITM repo = new Repo_OITM(logger);

        //    var list = repo.List();
        //    var result = JsonConvert.DeserializeObject<List<Producto>>(list);
        //    return result;
        //}

        public MensajeReturn Add(string item)
        {
            try
            {
                var oitm = JsonConvert.DeserializeObject<OITM>(item);
                Repo_OITM repo = new Repo_OITM(logger);
                //obtener correlativo familia

                var json = repo.GetCorrelativoFamilia(Convert.ToInt32(oitm.FamiliaCode));
                var correlativo = JsonConvert.DeserializeObject<spProducto_Correlativo_Result>(json);
                if (correlativo != null && correlativo.Correlativo > 0)
                {
                    Repo_ITM4 repofam = new Repo_ITM4();
                    json = repofam.Get(Convert.ToInt32(oitm.FamiliaCode));
                    var itm4 = JsonConvert.DeserializeObject<ITM4>(json);
                    oitm.ProdCode = String.Format("{0}{1}", oitm.ProdCode,  correlativo.Correlativo.ToString());

                    //GetCorrelativoFamilia
                    //crear primero en DF ------------------------
                    AgenteDefontana df = new AgenteDefontana(logger, cnndf);
                    var token = df.RenovarToken(cnndf.metodoauth);
                    bool succes = false;
                    ProductosDF proddf = new ProductosDF
                    {
                        code = oitm.ProdCode,
                        name = oitm.ProdNombre,
                        description = oitm.ProdNombre,
                        unit = oitm.Medida,
                        price = 0,
                        categoryID = null,
                        isService = false,
                        usaLotes = false
                    };

                    json = JsonConvert.SerializeObject(proddf, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });

                    string metodo = String.Format("{0}", "/api/sale/SaveProduct");
                    var result = df.ExecutePost(metodo, token, json, ref succes);

                    var prodresult = JsonConvert.DeserializeObject<SaveSaleReturn>(result);
                    if (prodresult != null && prodresult.success)
                    {
                        //crear en colonosweb --------------------


                        json = repo.Add(oitm);
                        var list = JsonConvert.DeserializeObject<Producto>(json);
                        MensajeReturn msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.OK;
                        msg.error = false;
                        msg.msg = "Add Producto";
                        msg.data = list;
                        return msg;
                    }
                    else
                    {
                        MensajeReturn msg = new MensajeReturn();
                        msg.statuscode = HttpStatusCode.InternalServerError;
                        msg.error = true;
                        if (prodresult != null)
                        {
                            msg.msg = prodresult.message;
                        }
                        else
                        {
                            msg.msg = "Error conexión al crear producto en Defontana";
                        }
                        msg.data = "";
                        return msg;
                    }
                }
                else
                {
                    MensajeReturn msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.InternalServerError;
                    msg.error = true;
                    msg.msg = String.Format("no fue posible ontener el correlativo familia: {0}", oitm.FamiliaCode);
                    if (correlativo != null)
                    {
                        msg.msg = String.Format("producto no puede ser creado, correlativo: {0}, familia: {1}", correlativo.Correlativo, oitm.FamiliaCode);
                    }
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

        public MensajeReturn Get(string prodcode)
        {
            try
            {

                Repo_OITM repo = new Repo_OITM(logger);

                var json =repo.Get(prodcode);
                var item = JsonConvert.DeserializeObject<Producto>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = item ==null ? "Producto no existe" : "Producto";
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


        public MensajeReturn ListCategorias()
        {

            try
            {
                MensajeReturn msg = new MensajeReturn { count = 0, error = false, msg = "", data = new List<item>() };

                AgenteDefontana df = new AgenteDefontana(logger, cnndf);
                var token = df.RenovarToken(cnndf.metodoauth);
                bool succes = false;
                //https://api.defontana.com/api/Sale/GetLegacyCategories
                string metodo = String.Format("{0}?status=0&itemsPerPage=250&pageNumber=1", "/api/Sale/GetLegacyCategories");
                var result = df.ExecuteGet(metodo, token, "", ref succes);

                var categdf = JsonConvert.DeserializeObject<itemsCategorias>(result);

                if (categdf != null && categdf.items.Any())
                {
                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.OK;
                    msg.count = categdf.items.Count();
                    msg.error = false;
                    msg.msg = "Categorias";
                    msg.data = categdf;
                    return msg;
                }

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
                var oitm = JsonConvert.DeserializeObject<OITM>(item);

                //crear primero en DF ------------------------
                AgenteDefontana df = new AgenteDefontana(logger, cnndf);
                var token = df.RenovarToken(cnndf.metodoauth);
                bool succes = false;
                ProductosDF proddf = new ProductosDF
                {
                    code = oitm.ProdCode,
                    name = oitm.ProdNombre,
                    description = oitm.ProdNombre,
                    unit = oitm.Medida,
                };

                var json = JsonConvert.SerializeObject(proddf, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                string metodo = String.Format("{0}", "/api/sale/UpdateProduct");
                var result = df.ExecutePost(metodo, token, json, ref succes);

                var prodresult = JsonConvert.DeserializeObject<SaveSaleReturn>(result);
                if (prodresult != null && prodresult.success)
                {

                    Repo_OITM repo = new Repo_OITM(logger);

                    json = repo.Modify(oitm);
                    var producto = JsonConvert.DeserializeObject<Producto>(json);
                    MensajeReturn msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.OK;
                    msg.error = false;
                    msg.msg = producto.ProdCode == null ? "Producto no existe" : "Actualizar Producto";
                    msg.data = producto;
                    return msg;
                }
                else
                {
                    MensajeReturn msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.InternalServerError;
                    msg.error = true;
                    msg.msg = prodresult.message;
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

        public MensajeReturn Delete(string prodcode)
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var item = repo.Delete(prodcode);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = item ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                msg.error = item;
                msg.msg = !item ? "Producto no existe" : "Producto";
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
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<Producto>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Productos";
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

        public MensajeReturn ListarPreciosFijos()
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<Producto>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Productos";
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

        public MensajeReturn UpdateStockDF()
        {
            try
            {
                //BODEGAPLANTAT
                MensajeReturn msg;
                logger.Info("UpdateStockDF...");
                AgenteDefontana df = new AgenteDefontana(logger, cnndf);

                var bods = this.ListBodegas();
                List<OBOD> data = bods.data;
                var list = new List<OITB>();
                logger.Info("df.RenovarToken(cnndf.metodoauth)...");
                var token = df.RenovarToken(cnndf.metodoauth);
                var listStockDF = new List<ProductosDF>();
                var listStockCostoDF = new List<OITB>();
                var listDF = new List<OITB>();
                logger.Info("token: {0}", token);
                if (token != "")
                {
                    foreach (var bodega in data)
                    {
                        try
                        {
                            bool succes = false;
                            string metodo = String.Format("{0}?storageID={1}&itemsPerPage=250&pageNumber=1", cnndf.metodostockbodega, bodega.BodegaDF);
                            logger.Info("df.ExecuteGet(metodo: {0}, token: {1})", metodo, token);
                            var result = df.ExecuteGet(metodo, token, "", ref succes);
                            logger.Info("UpdateStockDF... result: {0}", result);
                            var stockdf = JsonConvert.DeserializeObject<productListDF>(result);

                            if (stockdf.success)
                            {
                                int paginas = stockdf.totalItems / 250 + 1;
                                for (var i = 1; i <= paginas; i++)
                                {
                                    metodo = String.Format("{0}?storageID={1}&itemsPerPage={2}&pageNumber={3}", cnndf.metodostockbodega, bodega.BodegaDF, 250, i);
                                    result = df.ExecuteGet(metodo, token, "", ref succes);
                                    stockdf = JsonConvert.DeserializeObject<productListDF>(result);

                                    if (stockdf.success && stockdf.productList!=null)
                                    {
                                        foreach (var s in stockdf.productList)
                                        {
                                            //listStockDF.Add(s);
                                            var item = new OITB
                                            {
                                                ProdCode = s.productID,
                                                BodegaCode = bodega.BodegaCode,
                                                Stock = s.stock,
                                                Costo = 0
                                            };

                                            listStockCostoDF.Add(item);

                                            Debug.Print(s.productID);
                                        }
                                    }

                                }
                            }

                            
                            Repo_OITM repop = new Repo_OITM(logger);
                            //var listConStock = listStockCostoDF;//.FindAll(x => x.Stock > 0);
                            if (listStockCostoDF.Count > 0)
                            {
                                Repo_OITB repo = new Repo_OITB();
                                Repo_OITB_BASE repob = new Repo_OITB_BASE();
                                

                                var json = repob.List(bodega.BodegaCode);
                                var listBase = JsonConvert.DeserializeObject<List<OITB_BASE>>(json);
                                foreach (var s in listStockCostoDF)
                                {
                                    //metodo = String.Format("{0}?Code={1}&Date={2}", cnndf.metodocosto, s.ProdCode, String.Format("{0:yyyy-MM-dd}", DateTime.Now));
                                    //result = df.ExecuteGet(metodo, token, "", ref succes);
                                    //var costo = JsonConvert.DeserializeObject<CostoResult>(result);
                                    //s.Costo = costo.cost;

                                    //validar si produto esta en el maestro de productos -----
                                    json = repop.Get(s.ProdCode);
                                    var p = JsonConvert.DeserializeObject<OITM>(json);
                                    if(p==null)
                                    {
                                        //obtener producto desde DF y agregarlo al maestro de producto
                                        metodo = String.Format("{0}?status={1}&itemsPerPage={2}&pageNumber={3}&code={4}", "/api/Sale/GetProducts", 0, 250, 1,s.ProdCode);
                                        result = df.ExecuteGet(metodo, token, "", ref succes);
                                        var prodDf = JsonConvert.DeserializeObject<productListDF>(result);
                                        if(prodDf!=null && prodDf.productList.Any())
                                        {
                                            try
                                            {
                                                var prod = prodDf.productList[0];
                                                repop.Add(new OITM
                                                {
                                                    ProdCode = prod.code,
                                                    ProdNombre = prod.name,
                                                    Activo = "S",
                                                    Costo = s.Costo,
                                                    Medida = "KG",
                                                    TipoCode = prod.name.Contains("LC ") ? "B" : "A"
                                                });
                                            }
                                            catch(Exception exx)
                                            {
                                                logger.Error("{0}", exx.Message);
                                                logger.Error("{0}", exx.StackTrace);
                                            }
                                        }
                                    }
                                    //--------------------------------------------------------
                                    OITB_BASE stockBase = null;
                                    if (listBase != null)
                                    {
                                        stockBase = listBase.Find(x => x.ProdCode == s.ProdCode && x.BodegaCode == s.BodegaCode);
                                    }
                                    if (stockBase == null)
                                    {
                                        stockBase = new OITB_BASE { ProdCode = s.ProdCode, BodegaCode = s.BodegaCode, Stock = -1, Costo = s.Costo, Asignado = 0 };
                                        repob.Add(stockBase);
                                    }
                                    if (stockBase.Stock != s.Stock)
                                    {
                                        listDF.Add(s);
                                        json = repo.Get(s.ProdCode, s.BodegaCode);
                                        var oitb = JsonConvert.DeserializeObject<OITB>(json);
                                        if (oitb != null)
                                        {
                                            //obtener el costo actual que hay en DF ------
                                            metodo = String.Format("{0}?Code={1}&Date={2}", cnndf.metodocosto, s.ProdCode, String.Format("{0:yyyy-MM-dd}", DateTime.Now));
                                            result = df.ExecuteGet(metodo, token, "", ref succes);
                                            var costo = JsonConvert.DeserializeObject<CostoResult>(result);
                                            oitb.Costo = costo.cost;
                                            s.Costo = costo.cost;

                                            //actualizar OITM
                                            json = repop.Get(s.ProdCode);
                                            var prod = JsonConvert.DeserializeObject<OITM>(json);
                                            if (prod != null)
                                            {
                                                prod.Costo = s.Costo;
                                                repop.Modify(prod);
                                            }

                                            //--------------------------------------------

                                            oitb.Stock = s.Stock;
                                            repo.Modify(oitb);
                                        }
                                        else
                                        {
                                            repo.Add(new OITB { ProdCode = s.ProdCode, BodegaCode = s.BodegaCode, Stock = s.Stock, Costo = s.Costo, Asignado = 0, Ordenado = 0 });
                                        }

                                        
                                        stockBase.Stock = s.Stock;
                                        repob.Add(stockBase);
                                        logger.Info("Actualizando Stock. Producto: {0}, Bodega: {1} Stock: {2}, Costo: {3}", s.ProdCode, s.BodegaCode, s.Stock,s.Costo);
                                    }

                                }
                            }
                            //09-09-2024: activar los productos innactivos con stock
                            repop.ActivarProductosConStock();

                            listStockCostoDF = new List<OITB>();

                            
                            
                        }
                        catch (Exception ex)
                        {
                            logger.Error("UpdateStockDF...");
                            logger.Error("mensaje: {0}", ex.Message);
                            logger.Error("{0}", ex.StackTrace);
                        }
                    }
                }
                else
                {
                    logger.Warn("Actualiza Stock: {0}", "DF no retorna token");

                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.BadRequest;
                    msg.count = 0;
                    msg.error = true;
                    msg.msg = "DF no retorna token";
                    msg.data = "";
                    return msg;
                }
                //listStockCostoDF=listStockCostoDF.FindAll(x => x.Stock > 0).ToList();

                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = listDF.Count();
                msg.error = false;
                msg.msg = "Stock Actualizado";
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

        public MensajeReturn UpdateCostoDF()
        {
            try
            {
                //BODEGAPLANTAT
                MensajeReturn msg;
                AgenteDefontana df = new AgenteDefontana(logger, cnndf);

                var bods = this.ListBodegas();
                List<OBOD> data = bods.data;
                var list = new List<OITB>();
                var token = df.RenovarToken(cnndf.metodoauth);
                var listStockDF = new List<ProductosDF>();
                var listStockCostoDF = new List<OITB>();

                if (token != "")
                {
                    foreach (var bodega in data)
                    {
                        try
                        {
                            bool succes = false;
                            string metodo = String.Format("{0}?storageID={1}&itemsPerPage=250&pageNumber=1", cnndf.metodostockbodega, bodega.BodegaDF);
                            var result = df.ExecuteGet(metodo, token, "", ref succes);

                            var stockdf = JsonConvert.DeserializeObject<productListDF>(result);

                            if (stockdf.success)
                            {
                                int paginas = stockdf.totalItems / 250 + 1;
                                for (var i = 1; i <= paginas; i++)
                                {
                                    metodo = String.Format("{0}?storageID={1}&itemsPerPage={2}&pageNumber={3}", cnndf.metodostockbodega, bodega.BodegaDF, 250, i);
                                    result = df.ExecuteGet(metodo, token, "", ref succes);
                                    stockdf = JsonConvert.DeserializeObject<productListDF>(result);

                                    if (stockdf.success)
                                    {
                                        foreach (var s in stockdf.productList)
                                        {
                                            //listStockDF.Add(s);
                                            var item = new OITB
                                            {
                                                ProdCode = s.productID,
                                                BodegaCode = bodega.BodegaCode,
                                                Stock = s.stock,
                                                Costo = 0
                                            };
                                            metodo = String.Format("{0}?Code={1}&Date={2}", cnndf.metodocosto, s.productID, String.Format("{0:yyyy-MM-dd}", DateTime.Now));
                                            result = df.ExecuteGet(metodo, token, "", ref succes);
                                            var costo = JsonConvert.DeserializeObject<CostoResult>(result);
                                            item.Costo = costo.cost;
                                            listStockCostoDF.Add(item);

                                            Debug.Print(s.productID);
                                        }
                                    }

                                }
                            }

                            Repo_OITM repop = new Repo_OITM(logger);

                            var json = repop.List("S", "");
                            var listActivos = JsonConvert.DeserializeObject<List<OITM>>(json);

                            var listConStock = listStockCostoDF.FindAll(x => x.Costo > 0);
                            if (listConStock.Count > 0)
                            {
                                Repo_OITB repo = new Repo_OITB();
                                Repo_OITB_BASE repob = new Repo_OITB_BASE();


                                json = repob.List(bodega.BodegaCode);
                                var listBase = JsonConvert.DeserializeObject<List<OITB_BASE>>(json);

                                foreach (var p in listActivos)
                                {
                                    foreach (var s in listConStock.FindAll(x => x.ProdCode == p.ProdCode).ToList())
                                    {
                                        metodo = String.Format("{0}?Code={1}&Date={2}", cnndf.metodocosto, s.ProdCode, String.Format("{0:yyyy-MM-dd}", DateTime.Now));
                                        result = df.ExecuteGet(metodo, token, "", ref succes);
                                        var costo = JsonConvert.DeserializeObject<CostoResult>(result);
                                        s.Costo = costo.cost;

                                        OITB_BASE stockBase = null;
                                        if (listBase != null)
                                        {
                                            stockBase = listBase.Find(x => x.ProdCode == s.ProdCode && x.BodegaCode == s.BodegaCode);
                                        }
                                        if (stockBase == null)
                                        {
                                            stockBase = new OITB_BASE { ProdCode = s.ProdCode, BodegaCode = s.BodegaCode, Stock = 0, Costo = s.Costo, Asignado = 0 };
                                            repob.Add(stockBase);
                                        }
                                        if (Math.Round(Convert.ToDouble(stockBase.Costo), 2) != Math.Round(Convert.ToDouble(s.Costo), 2))
                                        {
                                            json = repo.Get(s.ProdCode, s.BodegaCode);
                                            var oitb = JsonConvert.DeserializeObject<OITB>(json);
                                            //oitb.Stock = s.Stock;
                                            oitb.Costo = s.Costo;
                                            repo.Modify(oitb);

                                            stockBase.Costo = s.Costo;
                                            repob.Add(stockBase);

                                            json = repop.Get(s.ProdCode);
                                            var prod = JsonConvert.DeserializeObject<OITM>(json);
                                            if (prod != null)
                                            {
                                                prod.Costo = s.Costo;
                                                repop.Modify(prod);
                                            }

                                            logger.Info("Actualizando Costo. Producto: {0}, Bodega: {1} Stock: {2} Costo: {3}", s.ProdCode, s.BodegaCode, s.Stock, s.Costo);
                                        }

                                    }
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            logger.Error("UpdateStockDF...");
                            logger.Error("mensaje: {0}", ex.Message);
                            logger.Error("{0}", ex.StackTrace);
                        }
                    }
                    //listStockCostoDF = listStockCostoDF.FindAll(x => x.Stock > 0).ToList();


                } else
                {
                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.BadRequest;
                    msg.count = 0;
                    msg.error = true;
                    msg.msg = "DF no retorna token";
                    msg.data = "";
                    return msg;
                }

                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = listStockCostoDF.Count();
                msg.error = false;
                msg.msg = "Costo Actualizado";
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

        public MensajeReturn UpdateStockDF(ConsultaStockDF listproductos, string bodegacodedf)
        {
            try
            {
                AgenteDefontana df = new AgenteDefontana(logger, cnndf);

                var token = df.RenovarToken(cnndf.metodoauth);
                var json = JsonConvert.SerializeObject(listproductos);
                bool succes = false;
                var metodo = String.Format("{0}?itemsPerPage=250&pageNumber=1", cnndf.metodostockgroup);
                var result = df.ExecutePost(metodo, token, json, ref succes);
                var stockdf = JsonConvert.DeserializeObject<productListDF>(result);
                var list = new List<OITB>();
                if(stockdf.success)
                {
                    Repo_OITB repo = new Repo_OITB();

                    var bods = this.ListBodegas();
                    List<OBOD> data = bods.data;
                    var bodega = data.Find(x => x.BodegaDF == bodegacodedf);
                    
                    stockdf.productList = stockdf.productList.FindAll(x => x.active=="S").ToList();

                    foreach (var s in stockdf.productList)
                    {
                        var st = s.stockDetail.Find(x => x.storageID == bodegacodedf);
                        json = repo.Get(s.code, bodega.BodegaCode);
                        var stock = JsonConvert.DeserializeObject<OITB>(json);
                        //consulta costo en DF
                        metodo = String.Format("{0}?Code={1}&Date={2}", cnndf.metodocosto, s.code,String.Format("{0:yyyy-MM-dd}",DateTime.Now));
                        result = df.ExecuteGet(metodo, token, "", ref succes);
                        var costo =  JsonConvert.DeserializeObject<CostoResult>(result);

                        stock.Stock = st.stock;
                        stock.Costo = costo.cost;
                        
                        repo.Modify(stock);
                        list.Add(stock);
                    }
                    
                }

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Productos";
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

        public MensajeReturn ListBodegas()
        {
            try
            {
                Repo_OBOD repo = new Repo_OBOD();

                var json = repo.List();
                var list = JsonConvert.DeserializeObject<List<OBOD>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Bodegas";
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
        public MensajeReturn List(string activo, bool maestro=true)
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.List(activo, maestro);
                var list = JsonConvert.DeserializeObject<List<ProductosResult>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Productos";
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
        public MensajeReturn List(string desglose)
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.List(desglose);
                var list = JsonConvert.DeserializeObject<List<Producto>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = list.Count();
                msg.error = false;
                msg.msg = "Listado Productos";
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

        public MensajeReturn List(string palabras, string solorecetas, string bodegacode)
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.List(palabras, solorecetas, bodegacode);
                var list = JsonConvert.DeserializeObject<List<ProductosResult>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado Productos";
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

        public MensajeReturn ListMaestro(string palabras, string solorecetas)
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.ListMaestro(palabras, solorecetas);
                var list = JsonConvert.DeserializeObject<List<ProductosResult>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado Productos";
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

        public MensajeReturn ListStock(string palabras)
        {
            try
            {
                Repo_OITM repo = new Repo_OITM(logger);

                var json = repo.ListStock(palabras);
                var list = JsonConvert.DeserializeObject<List<ProductosResult>>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = list.Count();
                msg.msg = "Listado Productos";
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

        public MensajeReturn GetStockBodega(string prodcode, string bodegacode)
        {
            try
            {
                Repo_OITB repo = new Repo_OITB();

                var json = repo.Get(prodcode,bodegacode);
                var stock = JsonConvert.DeserializeObject<OITB>(json);
                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = 0;
                msg.msg = "Stock Producto";
                msg.data = stock;
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

                Repo_ITM1 repo1 = new Repo_ITM1();
                Repo_ITM2 repo2 = new Repo_ITM2();
                Repo_ITM3 repo3 = new Repo_ITM3();
                Repo_ITM4 repo4 = new Repo_ITM4();
                Repo_ITM5 repo5 = new Repo_ITM5();
                Repo_ITM6 repo6 = new Repo_ITM6();
                Repo_ITM7 repo7 = new Repo_ITM7();
                Repo_ITM8 repo8 = new Repo_ITM8();
                Repo_ITM9 repo9 = new Repo_ITM9();
                Repo_ITM10 repo10 = new Repo_ITM10();

                
                var prop = new ProductoPropiedades();

                var json = repo1.List(); prop.umedidas = JsonConvert.DeserializeObject<List<ITM1>>(json);prop.umedidas.Add(new ITM1 { MedidaCode = "0", MedidaNombre = "" });
                json = repo2.List(); prop.categorias = JsonConvert.DeserializeObject<List<ITM2>>(json); prop.categorias.Add(new ITM2 { CategoriaCode="0",CategoriaNombre=""});
                json = repo3.List(); prop.tipos = JsonConvert.DeserializeObject<List<ITM3>>(json); prop.tipos.Add(new ITM3 {TipoCode="0",TipoNombre=""});
                json = repo4.List(); prop.familias = JsonConvert.DeserializeObject<List<ITM4>>(json); prop.familias.Add(new ITM4 {FamiliaCode=0,FamiliaNombre="" });
                json = repo5.List(); prop.animales = JsonConvert.DeserializeObject<List<ITM5>>(json); prop.animales.Add(new ITM5 {AnimalCode=0,AnimalNombre="" });
                json = repo6.List(); prop.formatos = JsonConvert.DeserializeObject<List<ITM6>>(json); prop.formatos.Add(new ITM6 { FormatoCode = 0,FormatoNombre="" }) ;
                json = repo7.List(); prop.refrigeracion = JsonConvert.DeserializeObject<List<ITM7>>(json); prop.refrigeracion.Add(new ITM7 {RefrigeraCode=0,RefrigeraNombre=""});
                json = repo8.List(); prop.formatoventa = JsonConvert.DeserializeObject<List<ITM8>>(json); prop.formatoventa.Add(new ITM8 {FrmtoVentaCode=0,FrmtoVentaNombre=""});
                json = repo9.List(); prop.marcas = JsonConvert.DeserializeObject<List<ITM9>>(json); prop.marcas.Add(new ITM9 {MarcaCode=0,MarcaNombre="" });
                json = repo10.List(); prop.origen = JsonConvert.DeserializeObject<List<ITM10>>(json); prop.origen.Add(new ITM10 {OrigenCode=0,OrigenNombre=""});

                var msgCat = this.ListCategorias();
                if (!msgCat.error)
                {
                    prop.categoriasDF = msgCat.data;
                }

                MensajeReturn msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.count = prop.umedidas.Count + 
                    prop.categorias.Count + 
                    prop.tipos.Count + 
                    prop.familias.Count + 
                    prop.animales.Count + 
                    prop.formatos.Count + 
                    prop.refrigeracion.Count + 
                    prop.formatoventa.Count +
                    prop.marcas.Count +
                    prop.origen.Count ;
                msg.msg = "Propiedades Productos";
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

        #region Defontana ****
        public MensajeReturn AjusteInventario(Transaccion item)
        {
            MensajeReturn msg = null;
            try
            {
                //Repo_OITM repo = new Repo_OITM(logger);

                //grabar en db Colonos

                Repo_OTRX repo = new Repo_OTRX();
                var json=repo.Add(item);
                var otrx = JsonConvert.DeserializeObject<Transaccion>(json);

                if (otrx.DocEntry > 0)
                {
                    Repo_OBOD repoBo = new Repo_OBOD();
                    json = repoBo.List();
                    var listBodegas = JsonConvert.DeserializeObject<List<OBOD>>(json);

                    var trx = new TransacciondeInventario
                    {
                        folio = 0,
                        fiscalYear = item.DocFecha.Year.ToString(),
                        gloss = "",
                        originStowageId = item.BodegaCodeOrigen=="" ? "" : listBodegas.Find(x => x.BodegaCode==item.BodegaCodeOrigen).BodegaDF,
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
                                if (otrx.BodegaCodeOrigen!="")
                                {
                                    json = repoInv.Get(i.ProdCode, otrx.BodegaCodeOrigen);
                                    stock = JsonConvert.DeserializeObject<OITB>(json);
                                    stock.Stock += i.CantidadSolicitada * -1;
                                    repoInv.Modify(stock);
                                }

                                if (otrx.BodegaCodeDestino != "")
                                {
                                    json = repoInv.Get(i.ProdCode, otrx.BodegaCodeDestino);
                                    stock = JsonConvert.DeserializeObject<OITB>(json);
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
                            msg.data = docdf;
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

        public MensajeReturn TraspasoInventario(string item)
        {
            //este metodo queda obsoleto, todo se hace en AjusteInventario()
            return new MensajeReturn(); 

            //try
            //{
            //    //Repo_OITM repo = new Repo_OITM(logger);

            //    var trx = JsonConvert.DeserializeObject<TransacciondeInventario>(item);
            //    var json = JsonConvert.SerializeObject(trx, Formatting.Indented);
            //    AgenteDefontana df = new AgenteDefontana(logger, cnndf);
            //    logger.Info("Ajuste inventario en DF: {0} \n {2}", trx.documentTypeId, json);

            //    var token = df.RenovarToken(cnndf.metodoauth);
            //    var result = "";
            //    bool succes = false;
            //    if (token != "")
            //    {
            //        result = df.ExecutePost("api/inventory/InsertSkipCentralization", token, json, ref succes);
            //    }
            //    //----------------------------------
            //    var docdf = JsonConvert.DeserializeObject<SaveSaleReturn>(result);
            //    if (succes)
            //    {
            //        if (docdf.success)
            //        {
            //            MensajeReturn msg = new MensajeReturn();
            //            msg.statuscode = HttpStatusCode.OK;
            //            msg.error = false;
            //            msg.msg = "Ajuste Realizado";
            //            msg.data = docdf;
            //            return msg;
            //        }
            //        else
            //        {
            //            MensajeReturn msg = new MensajeReturn();
            //            msg.statuscode = HttpStatusCode.BadRequest;
            //            msg.error = true;
            //            msg.msg = docdf.message;
            //            msg.data = docdf;
            //            return msg;
            //        }
            //    }
            //    else
            //    {
            //        MensajeReturn msg = new MensajeReturn();
            //        msg.statuscode = HttpStatusCode.BadRequest;
            //        msg.error = true;
            //        msg.msg = docdf.message;
            //        msg.data = docdf;
            //        return msg;
            //    }


            //}
            //catch (Exception ex)
            //{
            //    MensajeReturn msg = new MensajeReturn();
            //    msg.statuscode = HttpStatusCode.InternalServerError;
            //    msg.error = true;
            //    msg.msg = ex.Message;
            //    msg.data = ex.StackTrace;
            //    if (ex.InnerException != null)
            //    {
            //        msg.data += JsonConvert.SerializeObject(ex);
            //    }

            //    return msg;
            //}
        }
        #endregion
    }
}
