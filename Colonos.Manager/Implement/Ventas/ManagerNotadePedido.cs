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
using System.Transactions;

namespace Colonos.Manager
{
    public class ManagerNotadePedido
    {
        Logger logger;
        public ManagerNotadePedido(Logger _logger)
        {
            logger = _logger;
        }

        private List<BDA1> EvaluarCredito(Documento doc)
        {
            Repo_OSCP repocli = new Repo_OSCP(logger);
            Repo_SCP7 repoEst = new Repo_SCP7();
            var bda1List = new List<BDA1>();

            
            var json = repoEst.List();
            var estados = JsonConvert.DeserializeObject<List<SCP7>>(json);
            decimal creditodisponible = 0;
            var evaluacioncredito = "";
            json = repocli.Get(doc.SocioCode);
            var cli = JsonConvert.DeserializeObject<OSCP>(json);
            if (cli != null)
            {
                bool addbandejacredito = false;
                creditodisponible = cli.CreditoAutorizado ?? 0 - cli.CreditoUtiliado ?? 0; //sumar los pedidos que estan en proceso
                var estado = estados.Find(x => x.EstadoOperativo == cli.EstadoOperativo);
                doc.EstadoCliente = estado.Descripcion;

                

                if (doc.CondicionNombre.Contains("CONTADO")) //10-08-2024: jhernandez, Es contado, debe pasar por Bandeja siempre 
                {
                    evaluacioncredito = String.Format("Es una Venta Contado \n");
                    addbandejacredito = true;
                }
                else if ((estado.EstadoOperativo == "ACTIVO" || estado.EstadoOperativo == "ESPECIAL") && creditodisponible >= doc.Neto)
                {
                    //cliente autorizado para comprar ---
                    addbandejacredito = false;
                }
                //else if (doc.CondicionNombre.Contains("CONTADO")) //10-08-2024: jhernandez, Es contado, debe pasar por Bandeja siempre 
                //{
                //    evaluacioncredito = String.Format("Es una Venta Contado \n");
                //    addbandejacredito = true;
                //}
                else if (estado.EstadoOperativo == "BLOQUEO" || estado.EstadoOperativo == "MOROSIDAD" || estado.EstadoOperativo == "INACTIVO")
                {
                    evaluacioncredito += String.Format("Cliente en estado : {0} \n", estado.Descripcion);
                    addbandejacredito = true;
                }
                else if (creditodisponible <= 0) //No le queda credito disponible
                {
                    evaluacioncredito = String.Format("Cliente actualmente sin crédito disponible : {0:C0} \n", creditodisponible);
                    addbandejacredito = true;
                }
                else if (creditodisponible < doc.Neto) //Sobrepasa credito disponible
                {
                    evaluacioncredito += String.Format("Cliente sobre pasa crédito disponible actual: {0:C0} , solicitado: {1:C0} \n", creditodisponible, doc.Neto);
                    addbandejacredito = true;
                }

                //29-10-2024: hacer que todos los pedidos caigan en la badeja de credito hasta nuevo aviso ---
                addbandejacredito = true;
                evaluacioncredito += String.Format("**Todos los pedidos pasan por crédito** \n");
                //--------------------------------------------------------------------------------------------
                if (addbandejacredito)
                {
                    bda1List.Add(new BDA1
                    {
                        DocEntry = doc.DocEntry,
                        BandejaCode = "CRED",
                        DocLinea = 0,
                        Estado = false,
                        Autorizado = false,
                        FechaIngreso = doc.FechaRegistro,
                        UsuarioCodeIngreso = doc.UsuarioCode,
                        MotivoIngreso = evaluacioncredito,
                        ValorSolicitado = doc.Neto,
                        ValorRegla= creditodisponible,
                        MargenRegla=0
                    });
                }
            }
            return bda1List;
        }

        private List<BDA1> EvaluarPrecios(Documento doc)
        {
            Repo_LPC1 repoPrecio = new Repo_LPC1();
            Repo_ITM4 repoFamilia = new Repo_ITM4();
            Repo_ITM5 repoAnimal = new Repo_ITM5();
            Repo_OCFG_VTA repoConfig = new Repo_OCFG_VTA();

            //evaluar precios y margenes -------------------
            int lineaitem = 0;
            var json = "";
            var bda1List = new List<BDA1>();
            foreach (var p in doc.Lineas)
            {
                //lineaitem += 1;
                //p.LineaItem = lineaitem;
                decimal valorSolicitado = 0;
                decimal valorRegla = 0;
                var evaluacionprecio = "";
                var enviarbandejaprecio = false;

                //producto tiene precio fijo
                json = repoPrecio.Get(p.ProdCode);
                var preciofijo = JsonConvert.DeserializeObject<LPC1>(json);
                if (preciofijo != null && preciofijo.Precio != p.PrecioFinal && preciofijo.Desde <= DateTime.Now.Date && DateTime.Now.Date <= preciofijo.Hasta)
                {
                    ////no permitir generar pedido
                    //msg = new MensajeReturn();
                    //msg.statuscode = HttpStatusCode.BadRequest;
                    //msg.count = 1;
                    //msg.error = true;
                    //msg.msg = String.Format("Producto: {0} {1} tiene precio fijo: {2}", p.ProdCode, p.ProdNombre, preciofijo.Precio);
                    //msg.data = "";
                    //return msg;

                    //enviar a bandeja precio por precio fijo no permitido
                    evaluacionprecio = String.Format("{0}", "producto con precio Fijo");
                    enviarbandejaprecio = true;
                    valorSolicitado = Convert.ToDecimal(p.PrecioFinal);

                    //agregar a bandeja precio
                    if (enviarbandejaprecio)
                    {
                        bda1List.Add(new BDA1
                        {
                            DocEntry = p.DocEntry,
                            BandejaCode = "PREC",
                            DocLinea = p.DocLinea,
                            //LineaItem = p.LineaItem,
                            Estado = false,
                            Autorizado = false,
                            FechaIngreso = doc.FechaRegistro,
                            UsuarioCodeIngreso = doc.UsuarioCode,
                            MotivoIngreso = evaluacionprecio,
                            ValorSolicitado = valorSolicitado,
                            ValorRegla= preciofijo.Precio,
                            MargenRegla = 0
                        });
                    }
                }
                else if (preciofijo != null && preciofijo.Precio == p.PrecioFinal)
                {
                    //permitir pasar, cumple con el precio fijo
                    enviarbandejaprecio = false;
                }
                else
                {
                    //producto tiene margen precio familia
                    json = repoFamilia.Get(Convert.ToInt32(p.FamiliaCode));
                    var preciofamilia = JsonConvert.DeserializeObject<ITM4>(json);
                    if (preciofamilia != null && preciofamilia.Volumen != null && preciofamilia.Margen != 0)
                    {
                        var peciolista = Math.Round(Convert.ToDecimal(p.Costo / (1 - preciofamilia.Margen)), 0);
                        var peciolistavolumen = Math.Round(Convert.ToDecimal((p.Costo / (1 - preciofamilia.Margen)) * (1 - preciofamilia.DescVolumen)), 0);

                        if (p.CantidadSolicitada <= preciofamilia.Volumen && peciolista > p.PrecioFinal)
                        {
                            //enviar a bandeja precio por precio normal familia no permitido
                            evaluacionprecio = String.Format("{0}", "precio normal Familia no permitido");
                            enviarbandejaprecio = true;
                            valorSolicitado = Convert.ToDecimal(p.PrecioFinal);
                            valorRegla = Convert.ToDecimal(peciolista);
                        }
                        else if (p.CantidadSolicitada > preciofamilia.Volumen && peciolistavolumen > p.PrecioFinal)
                        {
                            //enviar a bandeja precio por precio volumen familia no permitido
                            evaluacionprecio = String.Format("{0}", "precio valumen Familia no permitido");
                            enviarbandejaprecio = true;
                            valorSolicitado = Convert.ToDecimal(p.PrecioFinal);
                            valorRegla = Convert.ToDecimal(peciolistavolumen);
                        }
                        else if (preciofamilia.Margen > p.Margen)
                        {
                            //enviar a bandeja precio por margen familia no permitido
                            evaluacionprecio = String.Format("{0}",  "margen Familia no permitido");
                            enviarbandejaprecio = true;
                            valorSolicitado = Convert.ToDecimal(p.Margen);
                            valorRegla = Convert.ToDecimal(preciofamilia.Margen);
                        }

                        //agregar a bandeja precio
                        if (enviarbandejaprecio)
                        {
                            bda1List.Add(new BDA1
                            {
                                DocEntry = p.DocEntry,
                                BandejaCode = "PREC",
                                DocLinea = p.DocLinea,
                                //LineaItem = p.LineaItem,
                                Estado = false,
                                Autorizado = false,
                                FechaIngreso = DateTime.Now,
                                UsuarioCodeIngreso = doc.UsuarioCode,
                                MotivoIngreso = evaluacionprecio,
                                ValorSolicitado = valorSolicitado,
                                ValorRegla = valorRegla,
                                MargenRegla= preciofamilia.Margen
                            });
                        }
                    }
                    else
                    {
                        //producto tiene margen precio animal
                        json = repoAnimal.Get(Convert.ToInt32(p.AnimalCode));
                        var precioanimal = JsonConvert.DeserializeObject<ITM5>(json);
                        if (precioanimal != null && precioanimal.Volumen != null && precioanimal.Margen != 0)
                        {
                            //var peciolista = p.Costo * precioanimal.FactorPrecio;
                            //var peciolistavolumen = p.Costo * precioanimal.FactorPrecio * (1 - precioanimal.DescVolumen);

                            var peciolista = Math.Round(Convert.ToDecimal(p.Costo / (1 - precioanimal.Margen)), 0);
                            var peciolistavolumen = Math.Round(Convert.ToDecimal((p.Costo / (1 - precioanimal.Margen)) * (1 - precioanimal.DescVolumen)), 0);

                            if (p.CantidadSolicitada <= precioanimal.Volumen && peciolista > p.PrecioFinal)
                            {
                                //enviar a bandeja precio por precio normal Animal no permitido
                                evaluacionprecio = String.Format("{0}", "precio normal Animal no permitido");
                                enviarbandejaprecio = true;
                                valorSolicitado = Convert.ToDecimal(p.PrecioFinal);
                                valorRegla =Convert.ToDecimal(peciolista);
                            }
                            else if (p.CantidadSolicitada > precioanimal.Volumen && peciolistavolumen > p.PrecioFinal)
                            {
                                //enviar a bandeja precio por precio volumen Animal no permitido
                                evaluacionprecio = String.Format("{0}", "precio volumen Animal no permitido");
                                enviarbandejaprecio = true;
                                valorSolicitado = Convert.ToDecimal(p.PrecioFinal);
                                valorRegla = Convert.ToDecimal(peciolistavolumen);
                            }
                            else if (precioanimal.Margen > p.Margen)
                            {
                                //enviar a bandeja precio por margen Animal no permitido
                                evaluacionprecio = String.Format("{0}", "margen Animal no permitido");
                                enviarbandejaprecio = true;
                                valorSolicitado = Convert.ToDecimal(p.Margen);
                                valorRegla = Convert.ToDecimal(precioanimal.Margen);
                            }
                            //agregar a bandeja precio
                            if (enviarbandejaprecio)
                            {
                                bda1List.Add(new BDA1
                                {
                                    DocEntry = p.DocEntry,
                                    BandejaCode = "PREC",
                                    DocLinea = p.DocLinea,
                                    //LineaItem = p.LineaItem,
                                    Estado = false,
                                    Autorizado = false,
                                    FechaIngreso = DateTime.Now,
                                    UsuarioCodeIngreso = doc.UsuarioCode,
                                    MotivoIngreso = evaluacionprecio,
                                    ValorSolicitado = valorSolicitado,
                                    ValorRegla=valorRegla,
                                    MargenRegla= precioanimal.Margen
                                });
                            }
                        }
                        else
                        {
                            json = repoConfig.Get(1);
                            var preciogeneral = JsonConvert.DeserializeObject<OCFG_VTA>(json);
                            if (preciogeneral != null && preciogeneral.Volumen != null && preciogeneral.Margen != 0)
                            {
                                var peciolista = Math.Round(Convert.ToDecimal(p.Costo / (1 - preciogeneral.Margen)), 0);
                                var peciolistavolumen = Math.Round(Convert.ToDecimal((p.Costo / (1 - preciogeneral.Margen)) * (1 - preciogeneral.DescVolumen)), 0);

                                if (p.CantidadSolicitada <= preciogeneral.Volumen && peciolista > p.PrecioFinal)
                                {
                                    //enviar a bandeja precio por precio general no permitido
                                    evaluacionprecio = String.Format("{0}", "precio general no permitido");
                                    enviarbandejaprecio = true;
                                    valorSolicitado = Convert.ToDecimal(p.PrecioFinal);
                                    valorRegla = Convert.ToDecimal(peciolista);
                                }
                                else if (p.CantidadSolicitada > preciogeneral.Volumen && peciolistavolumen > p.PrecioFinal)
                                {
                                    //enviar a bandeja precio por precio volumen no permitido
                                    evaluacionprecio = String.Format("{0}","precio volumen general no permitido");
                                    enviarbandejaprecio = true;
                                    valorSolicitado = Convert.ToDecimal(p.PrecioFinal);
                                    valorRegla = Convert.ToDecimal(peciolistavolumen);
                                }
                                else if (preciogeneral.Margen > p.Margen)
                                {
                                    //enviar a bandeja precio por margen no permitido
                                    evaluacionprecio = String.Format("{0}", "margen general no permitido");
                                    enviarbandejaprecio = true;
                                    valorSolicitado = Convert.ToDecimal(p.Margen);
                                    valorRegla = Convert.ToDecimal(preciogeneral.Margen);
                                }

                                //agregar a bandeja precio
                                if (enviarbandejaprecio)
                                {
                                    bda1List.Add(new BDA1
                                    {
                                        DocEntry = p.DocEntry,
                                        BandejaCode = "PREC",
                                        DocLinea = p.DocLinea,
                                        //LineaItem = p.LineaItem,
                                        Estado = false,
                                        Autorizado = false,
                                        FechaIngreso = DateTime.Now,
                                        UsuarioCodeIngreso = doc.UsuarioCode,
                                        MotivoIngreso = evaluacionprecio,
                                        ValorSolicitado = valorSolicitado,
                                        ValorRegla=valorRegla,
                                        MargenRegla= preciogeneral.Margen

                                    });
                                }
                            }
                        }
                    }

                    //producto tiene margen precio general
                }
            }
            //----------------------------------------------
            return bda1List;
        }

        public MensajeReturn ModifyEtiqueta(string item)
        {
            Documento doc = JsonConvert.DeserializeObject<Documento>(item);
            Repo_OPED repo = new Repo_OPED(logger);
            Repo_OPDC repopdc = new Repo_OPDC(logger);
            string json = "";
            if (doc.DocTipo == 10)
            {
                json = repo.Get(doc.DocEntry);
            }
            if (doc.DocTipo == 3010)
            {
                json = repopdc.Get(doc.DocEntry);
            }
            var pedActual = JsonConvert.DeserializeObject<Documento>(json);

            MensajeReturn msg = new MensajeReturn();
            if (pedActual.DocEstado == "A")
            {
                if (doc.DocTipo == 10)
                {
                    var oped = JsonConvert.DeserializeObject<OPED>(item);
                    json = repo.Modify(oped);
                }
                if (doc.DocTipo == 3010)
                {
                    var opdc = JsonConvert.DeserializeObject<OPDC>(item);
                    json = repopdc.Modify(opdc);
                }
                var ped = JsonConvert.DeserializeObject<Documento>(json);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Pedido no existe" : "Actualizar Pedido";
                msg.data = ped;

                return msg;
            }
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = true;
            msg.msg = String.Format("Pedido no puede ser modificado");
            msg.data = "";

            return msg;
        }

        public MensajeReturn ModifyOrdendeCompra(string item)
        {
            Documento doc = JsonConvert.DeserializeObject<Documento>(item);
            Repo_OPED repo = new Repo_OPED(logger);
            var json = repo.Get(doc.DocEntry);
            var pedActual = JsonConvert.DeserializeObject<Documento>(json);

            MensajeReturn msg = new MensajeReturn();
            if (pedActual.EstadoOperativo == "PREP")
            {
                var oped = JsonConvert.DeserializeObject<OPED>(item);

                json = repo.Modify(oped);
                var ped = JsonConvert.DeserializeObject<Documento>(json);
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Pedido no existe" : "Actualizar Pedido";
                msg.data = ped;

                return msg;
            }
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = true;
            msg.msg = String.Format("Pedido no puede ser modificado");
            msg.data = "";

            return msg;
        }
        public MensajeReturn Modify(string item)
        {
            
            Documento doc= JsonConvert.DeserializeObject<Documento>(item);

            Repo_OPED repo = new Repo_OPED(logger);
            Repo_PED1 repolin = new Repo_PED1(logger);
            Repo_OBDA repoBand = new Repo_OBDA();
            Repo_BDA1 repoBandLinea = new Repo_BDA1();

            var json = repo.Get(doc.DocEntry);
            var pedActual = JsonConvert.DeserializeObject<Documento>(json);


            MensajeReturn msg = new MensajeReturn();
            if (pedActual.EstadoOperativo == "ING")
            {
                var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var linupdate = doc.Lineas;


                UpdateDetalle(linupdate, linoriginal);
                doc = JsonConvert.DeserializeObject<Documento>(item);
                var oped = JsonConvert.DeserializeObject<OPED>(item);

                json = repo.Modify(oped);

                var ped = JsonConvert.DeserializeObject<Documento>(json);
                OBDA obda = null;

                //evaluar credito ------------------------------
                var listEvaluacion = EvaluarCredito(doc);
                if (listEvaluacion.Any())
                {
                    //grabar OBDA ***
                    
                    json=repoBand.Get("CRED", doc.DocEntry);
                    obda = JsonConvert.DeserializeObject<OBDA>(json);
                    if (obda == null)
                    {
                        obda = new OBDA
                        {
                            DocEntry = doc.DocEntry,
                            BandejaCode = "CRED",
                            Estado = false,
                            RazonSocial = ped.RazonSocial,
                            SocioCode = doc.SocioCode,
                            VendedorCode = doc.VendedorCode,
                            FechaIngreso = doc.FechaRegistro,
                            UsuarioCodeIngreso = doc.UsuarioNombre,
                            Visible = true


                        };
                        repoBand.Add(obda);
                    }
                    else
                    {
                        
                    }
                    

                    //Grabar Detalle ***
                    foreach (var b in listEvaluacion)
                    {
                        json = repoBandLinea.Get(b.DocEntry, b.BandejaCode, b.DocLinea);
                        var lineaActual = JsonConvert.DeserializeObject<BDA1>(json);
                        if (lineaActual == null)
                        {
                            //bda1List.Add(b);
                            repoBandLinea.Add(b);
                        }
                        else
                        {
                            if(b.ValorSolicitado!=lineaActual.ValorSolicitado)
                            {
                                json=repoBand.List(b.DocEntry);
                                var list = JsonConvert.DeserializeObject<List<OBDA>>(json);
                                foreach(var l in list)
                                {
                                    l.Estado = false;
                                    l.Autorizado = false;
                                    l.Visible = l.BandejaCode=="CRED" ? true : false;
                                    repoBand.Modify(l);
                                }

                                lineaActual.Estado = false;
                                lineaActual.Autorizado = false;
                                lineaActual.ValorSolicitado = b.ValorSolicitado;
                                repoBandLinea.Modify(lineaActual);

                                //json = repoBandLinea.List(b.BandejaCode, b.DocEntry);
                                //var lineas = JsonConvert.DeserializeObject<List<BDA1>>(json);
                                //foreach(var l in lineas)
                                //{
                                //    l.Estado = false;
                                //    l.Autorizado = false;
                                //    l.ValorSolicitado = b.ValorSolicitado;
                                //    repoBandLinea.Modify(l);
                                //}
                            }
                        }
                    }
                }
                //----------------------------------------------

                //evaluar precios y margenes -------------------
                listEvaluacion = EvaluarPrecios(doc);
                json = repoBandLinea.List("PREC", doc.DocEntry);
                var bandejaprecioactualdetalle = JsonConvert.DeserializeObject<List<BDA1>>(json);
                obda = null;
                json = repoBand.Get("PREC", doc.DocEntry);
                obda = JsonConvert.DeserializeObject<OBDA>(json);
                
                if (listEvaluacion.Any())
                {
                    //grabar OBDA ***
                   
                    if (obda == null)
                    {
                        obda = new OBDA
                        {
                            DocEntry = doc.DocEntry,
                            BandejaCode = "PREC",
                            Estado = false,
                            RazonSocial = doc.RazonSocial,
                            SocioCode = doc.SocioCode,
                            VendedorCode = doc.VendedorCode,
                            FechaIngreso = doc.FechaRegistro,
                            UsuarioCodeIngreso = doc.UsuarioNombre,
                            Visible = false
                        };
                        repoBand.Add(obda);
                    }
                    else
                    {

                    }

                    UpdateDetalle(listEvaluacion, bandejaprecioactualdetalle);

                    //foreach (var b in listEvaluacion)
                    //{
                    //    json = repoBandLinea.Get(b.DocEntry, b.BandejaCode, b.DocLinea);
                    //    var lineaActual = JsonConvert.DeserializeObject<BDA1>(json);
                    //    if (lineaActual == null)
                    //    {
                    //        //bda1List.Add(b);
                    //        repoBandLinea.Add(b);
                    //    }
                    //    else
                    //    {
                    //        if (b.ValorSolicitado != lineaActual.ValorSolicitado)
                    //        {
                    //            json = repoBand.List(b.BandejaCode, b.DocEntry);
                    //            var list = JsonConvert.DeserializeObject<List<OBDA>>(json);
                    //            foreach (var l in list)
                    //            {
                    //                l.Estado = false;
                    //                l.Autorizado = false;
                    //                //l.Visible = l.BandejaCode == "CRED" ? true : false;
                    //                repoBand.Modify(l);
                    //            }

                    //            lineaActual.Estado = false;
                    //            lineaActual.Autorizado = false;
                    //            lineaActual.ValorSolicitado = b.ValorSolicitado;
                    //            lineaActual.ValorRegla = b.ValorRegla;
                    //            lineaActual.MotivoIngreso = b.MotivoIngreso;
                    //            repoBandLinea.Modify(lineaActual);

                    //            //json = repoBandLinea.List(b.BandejaCode, b.DocEntry);
                    //            //var lineas = JsonConvert.DeserializeObject<List<BDA1>>(json);
                    //            //foreach (var l in lineas)
                    //            //{

                    //            //}
                    //        }
                    //    }
                    //}
                }
                else
                {
                    //validar si pedido ya tiene bandeja de precios, eliminar si existe
                    UpdateDetalle(listEvaluacion, bandejaprecioactualdetalle);
                    if (obda != null)
                    {
                        repoBand.Delete(obda.BandejaCode, obda.DocEntry);
                    }
                    
                    ManagerBandejas mngband = new ManagerBandejas(logger);
                    Documento docactual = JsonConvert.DeserializeObject<Documento>(item);

                    //validar si tiene bandejas pendientes
                    json = repoBand.Get("CRED", docactual.DocEntry);
                    obda = JsonConvert.DeserializeObject<OBDA>(json);
                    bool generarpreparacion = true;
                    if (docactual.EstadoOperativo == "ING")
                    {
                        if (obda != null && !Convert.ToBoolean(obda.Estado))
                        {
                            generarpreparacion = false;
                        }
                        else if (obda != null && !Convert.ToBoolean(obda.Estado) && !Convert.ToBoolean(obda.Autorizado))
                        {
                            generarpreparacion = false;
                        }
                        if (generarpreparacion)
                        {
                            json = repoBand.Get("PREC", docactual.DocEntry);
                            obda = JsonConvert.DeserializeObject<OBDA>(json);
                            if (obda != null && !Convert.ToBoolean(obda.Estado))
                            {
                                generarpreparacion = false;
                            }
                            else if (obda != null && !Convert.ToBoolean(obda.Estado) && !Convert.ToBoolean(obda.Autorizado))
                            {
                                generarpreparacion = false;
                            }
                        }
                        if (generarpreparacion)
                        {
                            mngband.GeneraPreparacionPedido(ref docactual);
                        }
                    }
                }

                ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Pedido no existe" : "Actualizar Pedido";
                msg.data = ped;

                return msg;
            }
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = true;
            msg.msg = String.Format( "Pedido no puede ser modificado. Estado Actual: {0}",pedActual.EstadoOperativo);
            msg.data = "";

            return msg;
        }

        public MensajeReturn ModifyEstadoItem(string item)
        {

            Documento doc = JsonConvert.DeserializeObject<Documento>(item);
            Repo_OITB repostock = new Repo_OITB();
            Repo_OPED repo = new Repo_OPED(logger);
            Repo_PED1 repolin = new Repo_PED1(logger);
            Repo_OBDA repoBand = new Repo_OBDA();
            Repo_BDA1 repoBandLinea = new Repo_BDA1();

            var json = repo.Get(doc.DocEntry);
            var pedActual = JsonConvert.DeserializeObject<Documento>(json);


            MensajeReturn msg = new MensajeReturn();
            if (pedActual.EstadoOperativo == "PREP")
            {
                var linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var linupdate = doc.Lineas;

                bool tienecantidadmarcada = false;
                foreach(var d in linupdate)
                {
                    var linea = linoriginal.Find(x => x.LineaItem == d.LineaItem);
                    if(linea!=null && linea.CantidadReal>0 && linea.LineaEstado=="A")
                    {
                        tienecantidadmarcada = true;
                        break;
                    }
                }
                if(tienecantidadmarcada)
                {
                    msg = new MensajeReturn();
                    msg.statuscode = HttpStatusCode.OK;
                    msg.error = true;
                    msg.msg ="Pedido tiene Items con cantidad marcada por facturar";
                    msg.data = "";

                    return msg;
                }
                UpdateDetalle(linupdate, linoriginal);
                var oped = JsonConvert.DeserializeObject<OPED>(item);

                linoriginal = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                var cerrarpedido = true;
                foreach (var i in linoriginal)
                {
                    if(i.LineaEstado=="A")
                    {
                        cerrarpedido = false;
                    }
                    else
                    {
                        json = repostock.Get(i.ProdCode, i.BodegaCode);
                        var oitb = JsonConvert.DeserializeObject<OITB>(json);
                        if (oitb != null)
                        {
                            oitb.Asignado -= i.CantidadPendiente;
                            repostock.Modify(oitb);
                        }
                    }
                }
                if(cerrarpedido)
                {
                    oped.DocEstado = "C";
                }
                json = repo.Modify(oped);

                var ped = JsonConvert.DeserializeObject<Documento>(json);
               
                ped.Lineas = JsonConvert.DeserializeObject<List<DocumentoLinea>>(repolin.List(doc.DocEntry));
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.error = false;
                msg.msg = ped == null ? "Pedido no existe" : "Actualizar Pedido";
                msg.data = ped;

                return msg;
            }
            msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = true;
            msg.msg = String.Format("Pedido no puede ser modificado. Estado Actual: {0}", pedActual.EstadoOperativo);
            msg.data = "";

            return msg;
        }
        public MensajeReturn Add(Documento doc)
        {
            MensajeReturn msg;
            try
            {

              
                Repo_OPED repo = new Repo_OPED(logger);
                Repo_LPC1 repoPrecio = new Repo_LPC1();
                Repo_ITM4 repoFamilia = new Repo_ITM4();
                Repo_ITM5 repoAnimal= new Repo_ITM5();
                
                Repo_OSCP repocli = new Repo_OSCP(logger);
                Repo_OBDA repoBand = new Repo_OBDA();
                Repo_BDA1 repoBandLinea = new Repo_BDA1();

                //evaluar credito  -----------------------------
                //cliente activo y morosidad ok-> no enviar a bandeja
                //cliente con mas de 3 dias de morosidad -> enviar a bandeja por BLOQUEO MOROSIDAD
                //cliente no tiene cupo disponible -> enviar a bandeja por BLOQUEO CUPO
                //cliente en estado BETADO no permitir la venta
                

                var bda1List = new List<BDA1>();

                var repoEst = new Repo_SCP7();
                var json = repoEst.List();
                var estados = JsonConvert.DeserializeObject<List<SCP7>>(json);

                Documento docgenerado;
                //using (TransactionScope transaction = new TransactionScope())
                //{

                doc.FechaRegistro = DateTime.Now;

                json = repo.Add(doc);
                
                    docgenerado = JsonConvert.DeserializeObject<Documento>(json);
                    doc.DocEntry = docgenerado.DocEntry;
                    docgenerado.Bandejas = new List<DocumentoLineaBandeja>();
                    docgenerado.DocumentoReferencia = new List<Documento>();
                    //evaluar credito ------------------------------
                    bool tienebandejacredito = false;
                    var listEvaluacion = EvaluarCredito(doc);
                    if (listEvaluacion.Any())
                    {
                        //grabar OBDA ***
                        var obda = new OBDA
                        {
                            DocEntry = doc.DocEntry,
                            BandejaCode = "CRED",
                            Estado = false,
                            RazonSocial = docgenerado.RazonSocial,
                            SocioCode = doc.SocioCode,
                            VendedorCode = doc.VendedorCode,
                            FechaIngreso = doc.FechaRegistro,
                            UsuarioCodeIngreso = doc.UsuarioNombre,
                            Visible = true


                        };
                        repoBand.Add(obda);

                        //Grabar Detalle ***
                        foreach (var b in listEvaluacion)
                        {
                            //bda1List.Add(b);
                            repoBandLinea.Add(b);
                            json = JsonConvert.SerializeObject(b);
                            var bb = JsonConvert.DeserializeObject<DocumentoLineaBandeja>(json);
                            docgenerado.Bandejas.Add(bb);
                        }

                        tienebandejacredito = true;

                    }
                    //----------------------------------------------
                    //evaluar precios y margenes -------------------
                    listEvaluacion = EvaluarPrecios(docgenerado);
                    if (listEvaluacion.Any())
                    {
                        //grabar OBDA ***
                        var obda = new OBDA
                        {
                            DocEntry = doc.DocEntry,
                            BandejaCode = "PREC",
                            Estado = false,
                            RazonSocial = doc.RazonSocial,
                            SocioCode = doc.SocioCode,
                            VendedorCode = doc.VendedorCode,
                            FechaIngreso = doc.FechaRegistro,
                            UsuarioCodeIngreso = doc.UsuarioNombre,
                            Visible = !tienebandejacredito
                        };
                        repoBand.Add(obda);
                        foreach (var b in listEvaluacion)
                        {
                            //bda1List.Add(b);
                            repoBandLinea.Add(b);
                            json = JsonConvert.SerializeObject(b);
                            var bb = JsonConvert.DeserializeObject<DocumentoLineaBandeja>(json);
                            docgenerado.Bandejas.Add(bb);
                        }
                    }

                    if (!docgenerado.Bandejas.Any())
                    {
                        ManagerBandejas mngband = new ManagerBandejas(logger);
                        mngband.GeneraPreparacionPedido(ref docgenerado);
                    }
                //    transaction.Complete();
                //}
                msg = new MensajeReturn();
                msg.statuscode = HttpStatusCode.OK;
                msg.count = 1;
                msg.error = false;
                msg.msg = "Pedido de Venta";
                msg.data = docgenerado;
                return msg;


                #region Comentarios1 ****
                //decimal creditodisponible = 0;

                //var evaluacioncredito = "";
                //json = repocli.Get(doc.SocioCode);
                //var cli = JsonConvert.DeserializeObject<OSCP>(json);
                //if (cli != null)
                //{
                //    bool addbandejacredito = false;
                //    creditodisponible = cli.CreditoAutorizado ?? 0 - cli.CreditoUtiliado ?? 0;
                //    var estado = estados.Find(x => x.EstadoOperativo == cli.EstadoOperativo);
                //    doc.EstadoCliente = estado.Descripcion;

                //    if ((estado.EstadoOperativo == "ACTIVO" || estado.EstadoOperativo == "ESPECIAL") && creditodisponible >= doc.Neto)
                //    {
                //        //cliente autorizado para comprar ---
                //        addbandejacredito = false;
                //    }
                //    else if (creditodisponible <= 0) //No le queda credito disponible
                //    {
                //        evaluacioncredito = String.Format("Cliente actualmente sin crédito disponible : {0:C0} \n", creditodisponible);
                //        addbandejacredito = true;
                //        bda1List.Add(new BDA1 { 
                //            DocEntry = 0, 
                //            BandejaCode = "CRED", 
                //            DocLinea = 0, 
                //            LineaItem=0,
                //            Estado = false, 
                //            Autorizado = false, 
                //            FechaIngreso = DateTime.Now, 
                //            UsuarioCodeIngreso = doc.UsuarioCode,
                //            MotivoIngreso= evaluacioncredito,
                //            ValorSolicitado = doc.Neto
                //        });
                //    }
                //    else if (creditodisponible < doc.Neto) //Sobrepasa credito disponible
                //    {
                //        evaluacioncredito += String.Format("Cliente sobre pasa crédito disponible actual: {0:C0} , solicitado: {1:C0} \n", creditodisponible, doc.Neto);
                //        addbandejacredito = true;
                //        bda1List.Add(new BDA1
                //        {
                //            DocEntry = 0,
                //            BandejaCode = "CRED",
                //            DocLinea = 0,
                //            Estado = false,
                //            Autorizado = false,
                //            FechaIngreso = DateTime.Now,
                //            UsuarioCodeIngreso = doc.UsuarioCode,
                //            MotivoIngreso = evaluacioncredito,
                //            ValorSolicitado= doc.Neto
                //        });
                //    }
                //    else if (estado.EstadoOperativo == "BLOQUEO" || estado.EstadoOperativo == "MORISIDAD" || estado.EstadoOperativo == "INACTIVO")
                //    {

                //        evaluacioncredito += String.Format("Cliente en estado : {0} \n", estado.Descripcion);
                //        addbandejacredito = true;
                //        bda1List.Add(new BDA1
                //        {
                //            DocEntry = 0,
                //            BandejaCode = "CRED",
                //            DocLinea = 0,
                //            Estado = false,
                //            Autorizado = false,
                //            FechaIngreso = DateTime.Now,
                //            UsuarioCodeIngreso = doc.UsuarioCode,
                //            MotivoIngreso = evaluacioncredito,
                //            ValorSolicitado = doc.Neto
                //        });
                //    }
                //}
                //----------------------------------------------

                #endregion ***

                #region comentario2 
                //int lineaitem = 0;
                //foreach(var p in doc.Lineas)
                //{
                //    lineaitem += 1;
                //    p.LineaItem = lineaitem;
                //    decimal valorSolicitado = 0;
                //    var evaluacionprecio = "";
                //    var enviarbandejaprecio = false;

                //    //producto tiene precio fijo
                //    json = repoPrecio.Get(p.ProdCode);
                //    var preciofijo=JsonConvert.DeserializeObject<LCP1>(json);
                //    if (preciofijo != null && preciofijo.Precio != p.PrecioFinal && preciofijo.Desde >= DateTime.Now.Date && DateTime.Now.Date <= preciofijo.Hasta)
                //    {
                //        ////no permitir generar pedido
                //        //msg = new MensajeReturn();
                //        //msg.statuscode = HttpStatusCode.BadRequest;
                //        //msg.count = 1;
                //        //msg.error = true;
                //        //msg.msg = String.Format("Producto: {0} {1} tiene precio fijo: {2}", p.ProdCode, p.ProdNombre, preciofijo.Precio);
                //        //msg.data = "";
                //        //return msg;

                //        //enviar a bandeja precio por precio volumen familia no permitido
                //        evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio con precio Fijo");
                //        enviarbandejaprecio = true;
                //        valorSolicitado = p.PrecioFinal;
                //    }
                //    else
                //    {
                //        //producto tiene margen precio familia
                //        json = repoFamilia.Get(p.FamiliaCode);
                //        var preciofamilia = JsonConvert.DeserializeObject<ITM4>(json);
                //        if (preciofamilia!=null && preciofamilia.FactorPrecio!=0 && preciofamilia.Volumen!=0 && preciofamilia.Margen!=0)
                //        {
                //            var peciolista = p.Costo * preciofamilia.FactorPrecio;
                //            var peciolistavolumen = p.Costo * preciofamilia.FactorPrecio * (1 - preciofamilia.DescVolumen);
                //            if (p.CantidadSolitada <= preciofamilia.Volumen && peciolista > p.PrecioFinal)
                //            {
                //                //enviar a bandeja precio por precio normal familia no permitido
                //                evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}",p.ProdCode,p.ProdNombre, "precio normal Familia no permitido");
                //                enviarbandejaprecio = true;
                //                valorSolicitado = p.PrecioFinal;
                //            }
                //            else if (p.CantidadSolitada > preciofamilia.Volumen && peciolistavolumen > p.PrecioFinal)
                //            {
                //                //enviar a bandeja precio por precio volumen familia no permitido
                //                evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio valumen Familia no permitido");
                //                enviarbandejaprecio = true;
                //                valorSolicitado = p.PrecioFinal;
                //            }
                //            else if (preciofamilia.Margen > p.Margen)
                //            {
                //                //enviar a bandeja precio por margen familia no permitido
                //                evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio por margen Familia no permitido");
                //                enviarbandejaprecio = true;
                //                valorSolicitado = p.Margen;
                //            }

                //            //agregar a bandeja precio
                //            if (enviarbandejaprecio)
                //            {
                //                bda1List.Add(new BDA1
                //                {
                //                    DocEntry = 0,
                //                    BandejaCode = "PREC",
                //                    DocLinea = p.DocLinea,
                //                    LineaItem = p.LineaItem,
                //                    Estado = false,
                //                    Autorizado = false,
                //                    FechaIngreso = DateTime.Now,
                //                    UsuarioCodeIngreso = doc.UsuarioCode,
                //                    MotivoIngreso = evaluacionprecio,
                //                    ValorSolicitado= valorSolicitado
                //                });
                //            }
                //        }
                //        else
                //        {
                //            //producto tiene margen precio animal
                //            json = repoAnimal.Get(p.AnimalCode);
                //            var precioanimal = JsonConvert.DeserializeObject<ITM5>(json);
                //            if (precioanimal != null && precioanimal.FactorPrecio != 0 && precioanimal.Volumen != 0 && precioanimal.Margen != 0)
                //            {
                //                var peciolista = p.Costo * precioanimal.FactorPrecio;
                //                var peciolistavolumen = p.Costo * precioanimal.FactorPrecio * (1 - precioanimal.DescVolumen);
                //                if (p.CantidadSolitada<=precioanimal.Volumen && peciolista>p.PrecioFinal)
                //                {
                //                    //enviar a bandeja precio por precio normal Animal no permitido
                //                    evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio normal Animal no permitido");
                //                    enviarbandejaprecio = true;
                //                    valorSolicitado = p.PrecioFinal;
                //                }
                //                else if (p.CantidadSolitada > precioanimal.Volumen && peciolistavolumen > p.PrecioFinal)
                //                {
                //                    //enviar a bandeja precio por precio volumen Animal no permitido
                //                    evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio volumen Animal no permitido");
                //                    enviarbandejaprecio = true;
                //                    valorSolicitado = p.PrecioFinal;
                //                }
                //                else if (precioanimal.Margen > p.Margen)
                //                {
                //                    //enviar a bandeja precio por margen Animal no permitido
                //                    evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio por margen Animal no permitido");
                //                    enviarbandejaprecio = true;
                //                    valorSolicitado = p.Margen;
                //                }
                //                //agregar a bandeja precio
                //                if (enviarbandejaprecio)
                //                {
                //                    bda1List.Add(new BDA1
                //                    {
                //                        DocEntry = 0,
                //                        BandejaCode = "PREC",
                //                        DocLinea = p.DocLinea,
                //                        LineaItem = p.LineaItem,
                //                        Estado = false,
                //                        Autorizado = false,
                //                        FechaIngreso = DateTime.Now,
                //                        UsuarioCodeIngreso = doc.UsuarioCode,
                //                        MotivoIngreso = evaluacionprecio,
                //                        ValorSolicitado = valorSolicitado
                //                    });
                //                }
                //            }
                //            else
                //            {
                //                json = repoConfig.Get(1);
                //                var preciogeneral = JsonConvert.DeserializeObject<OCFG_VTA>(json);
                //                if (preciogeneral != null && preciogeneral.FactorPrecio != 0 && preciogeneral.Volumen != 0 && preciogeneral.Margen != 0)
                //                {
                //                    var peciolista = p.Costo * preciogeneral.FactorPrecio;
                //                    var peciolistavolumen = p.Costo * preciogeneral.FactorPrecio * (1 - preciogeneral.DescVolumen);
                //                    if (p.CantidadSolitada <= preciogeneral.Volumen && peciolista > p.PrecioFinal)
                //                    {
                //                        //enviar a bandeja precio por precio general no permitido
                //                        evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio general no permitido");
                //                        enviarbandejaprecio = true;
                //                        valorSolicitado = p.PrecioFinal;
                //                    }
                //                    else if (p.CantidadSolitada > preciogeneral.Volumen && peciolistavolumen > p.PrecioFinal)
                //                    {
                //                        //enviar a bandeja precio por precio volumen no permitido
                //                        evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio volumen general no permitido");
                //                        enviarbandejaprecio = true;
                //                        valorSolicitado = p.PrecioFinal;
                //                    }
                //                    else if (preciogeneral.Margen > p.Margen)
                //                    {
                //                        //enviar a bandeja precio por margen no permitido
                //                        evaluacionprecio = String.Format("Producto: {0} {1} motivo: {2}", p.ProdCode, p.ProdNombre, "precio margen general no permitido");
                //                        enviarbandejaprecio = true;
                //                        valorSolicitado = p.Margen;
                //                    }

                //                    //agregar a bandeja precio
                //                    if (enviarbandejaprecio)
                //                    {
                //                        bda1List.Add(new BDA1
                //                        {
                //                            DocEntry = 0,
                //                            BandejaCode = "PREC",
                //                            DocLinea = p.DocLinea,
                //                            LineaItem = p.LineaItem,
                //                            Estado = false,
                //                            Autorizado = false,
                //                            FechaIngreso = DateTime.Now,
                //                            UsuarioCodeIngreso = doc.UsuarioCode,
                //                            MotivoIngreso = evaluacionprecio,
                //                            ValorSolicitado= valorSolicitado,
                //                        });
                //                    }
                //                }
                //            }
                //        }

                //        //producto tiene margen precio general
                //    }
                //}
                ////----------------------------------------------
                #endregion



                //var docgenerado = JsonConvert.DeserializeObject<Documento>(json);



                //docgenerado.Bandejas = new List<DocumentoLineaBandeja>();
                //if (docgenerado != null && docgenerado.DocEntry>0)
                //{
                //    //grabar bandeja de autorizacion ------
                //    if(bda1List.Any())
                //    {
                //        obda.DocEntry = docgenerado.DocEntry;
                //        obda.FechaIngreso = docgenerado.FechaRegistro;
                //        obda.Estado = false;
                //        obda.UsuarioCodeIngreso=docgenerado.UsuarioCode;
                //        repoBand.Add(obda);
                //        foreach(var b in bda1List)
                //        {
                //            b.DocEntry = docgenerado.DocEntry;
                //            if (b.LineaItem != null)
                //            {
                //                b.DocLinea = docgenerado.Lineas.Find(x => x.LineaItem == b.LineaItem).DocLinea;
                //            }
                //            repoBandLinea.Add(b);
                //            json = JsonConvert.SerializeObject(b);
                //            var bb = JsonConvert.DeserializeObject<DocumentoLineaBandeja>(json);
                //            docgenerado.Bandejas.Add(bb);
                //        }
                //    }
                //}


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

        public MensajeReturn Search(string palabras, string vendedocode, string usuario)
        {
            Repo_OPED repo = new Repo_OPED(logger);

            var json = repo.Search(palabras, vendedocode, usuario);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Pedidos";
            msg.data = list;
            return msg;
        }

        public MensajeReturn List()
        {
            Repo_OPED repo = new Repo_OPED(logger);
            var json = repo.List();
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Bandeja Bodega";
            msg.data = list;
            return msg;
        }

        public MensajeReturn List(string vendedorcode, string estadooperativo, string estado, int pendiente, string fechaini, string fechafin, string bodegacode,string usuario)
        {
            Repo_OPED repo = new Repo_OPED(logger);
            var json = repo.List(vendedorcode, estadooperativo, estado, pendiente, fechaini, fechafin, bodegacode);
            var list = JsonConvert.DeserializeObject<List<DocumentosResult>>(json);
            //if(usuario!="")
            //{
            //    list = list.FindAll(x => x.VendedorCode == usuario);
            //}
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = list.Count();
            msg.msg = "Listado Bandeja Bodega";
            msg.data = list;
            return msg;
        }

        public MensajeReturn Get(int docentry)
        {
            Repo_OPED repo = new Repo_OPED(logger);
            //Repo_PED1 repolin = new Repo_PED1(logger);

            var json = repo.Get(docentry);
            var doc = JsonConvert.DeserializeObject<Documento>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Pedido no existe" : "Pedido";
            msg.data = doc;

            return msg;
        }

        public MensajeReturn Historial(int docentry)
        {
            Repo_OPED repo = new Repo_OPED(logger);
            //Repo_PED1 repolin = new Repo_PED1(logger);

            var json = repo.Historial(docentry);
            var doc = JsonConvert.DeserializeObject<List<HistorialDoc>>(json);
            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.error = false;
            msg.count = 1;
            msg.msg = doc == null ? "Pedido no existe" : "Pedido";
            msg.data = doc;

            return msg;
        }
        private void UpdateDetalle(List<DocumentoLinea> ItemsUpdate, List<DocumentoLinea> ItemsCurr)
        {
            Repo_PED1 repo = new Repo_PED1(logger);
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (DocumentoLinea ilin in ItemsUpdate)
                    {

                        json = JsonConvert.SerializeObject(ilin);
                        var lin = JsonConvert.DeserializeObject<PED1>(json);
                        repo.Add(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count == 0)
                {
                    foreach (var i in ItemsCurr)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PED1>(json);
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
                            var lin = JsonConvert.DeserializeObject<PED1>(json);
                            repo.Modify(lin);
                            ItemsUpdateCopy.Remove(cd);
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(i);
                            var lin = JsonConvert.DeserializeObject<PED1>(json);
                            repo.Delete(lin);
                        }
                    }

                    foreach (var i in ItemsUpdateCopy)
                    {
                        json = JsonConvert.SerializeObject(i);
                        var lin = JsonConvert.DeserializeObject<PED1>(json);
                        repo.Add(lin);
                    }
                }
            }
        }

        private void UpdateDetalle(List<BDA1> ItemsUpdate, List<BDA1> ItemsCurr)
        {
            Repo_BDA1 repo = new Repo_BDA1();
            var json = "";
            if (ItemsUpdate != null && ItemsCurr != null)
            {
                if (ItemsCurr.Count == 0 && ItemsUpdate.Count > 0)
                {
                    foreach (BDA1 ilin in ItemsUpdate)
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
                        repo.Delete(lin);
                    }
                }
                else if (ItemsCurr.Count > 0 && ItemsUpdate.Count > 0)
                {
                    List<BDA1> ItemsUpdateCopy = ItemsUpdate;
                    foreach (var i in ItemsCurr)
                    {
                        BDA1 cd = ItemsUpdate.Find(x => x.DocLinea == i.DocLinea);
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
                            repo.Delete(lin);
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
    }
}
