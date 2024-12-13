using Colonos.Entidades.Defontana;
using Colonos.Manager;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Colonos.ActualizaStock
{
    partial class ActualizaStockServicio : ServiceBase
    {
        Logger logger = NLog.LogManager.GetLogger("loggerfile2");
        System.Timers.Timer timerVtex = new System.Timers.Timer();
        public ActualizaStockServicio()
        {
            InitializeComponent();
            int interval = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Interval"));
            timerVtex.Interval = 60000 * interval;
            timerVtex.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timerVtex.Enabled = true;
            timerVtex.Start();
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            timerVtex.Enabled = false; 
            timerVtex.Stop();
            logger.Info("Iniciado proceso actualiza Stock");
            try
            {
                var cnndf = setCnnDF();
                ManagerProductos mng = new ManagerProductos(logger, cnndf);
                var item = mng.UpdateStockDF();
            }
            catch(Exception ex)
            {
                logger.Error("{0}", ex.Message);
                logger.Error("{0}", ex.StackTrace);
            }
            finally
            {
                timerVtex.Enabled = true;
                timerVtex.Start();
                logger.Info("Finalizado proceso actualiza Stock");
            }
        }

        private cnnDF setCnnDF()
        {
            var cnndf = new cnnDF();
            cnndf.baseurl = ConfigurationManager.AppSettings.Get("baseurl");
            cnndf.cliente = ConfigurationManager.AppSettings.Get("cliente");
            cnndf.company = ConfigurationManager.AppSettings.Get("company");
            cnndf.user = ConfigurationManager.AppSettings.Get("user");
            cnndf.pass = ConfigurationManager.AppSettings.Get("pass");
            cnndf.docfactura = ConfigurationManager.AppSettings.Get("docfactura");
            cnndf.docingreso = ConfigurationManager.AppSettings.Get("docingreso");
            cnndf.docegreso = ConfigurationManager.AppSettings.Get("docegreso");
            cnndf.doctraslado = ConfigurationManager.AppSettings.Get("doctraslado");
            cnndf.metodoajustes = ConfigurationManager.AppSettings.Get("metodoajustes");
            cnndf.metodotraslados = ConfigurationManager.AppSettings.Get("metodotraslados");
            cnndf.metodofacturas = ConfigurationManager.AppSettings.Get("metodofacturas");
            cnndf.metodostockgroup = ConfigurationManager.AppSettings.Get("metodostockgroup");
            cnndf.metodostockbodega = ConfigurationManager.AppSettings.Get("metodostockbodega");
            cnndf.metodocosto = ConfigurationManager.AppSettings.Get("metodocosto");
            cnndf.accclientesnacionales = ConfigurationManager.AppSettings.Get("accclientesnacionales");
            cnndf.acciva = ConfigurationManager.AppSettings.Get("acciva");
            cnndf.valoriva = Convert.ToDecimal(ConfigurationManager.AppSettings.Get("valoriva"));
            cnndf.metodoauth = ConfigurationManager.AppSettings.Get("metodoauth");
            cnndf.accventaingresos = ConfigurationManager.AppSettings.Get("accventaingresos");

            return cnndf;
        }
        protected override void OnStart(string[] args)
        {
            // TODO: agregar código aquí para iniciar el servicio.
        }

        protected override void OnStop()
        {
            // TODO: agregar código aquí para realizar cualquier anulación necesaria para detener el servicio.
        }
    }
}
