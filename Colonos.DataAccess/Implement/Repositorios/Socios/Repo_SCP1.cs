using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_SCP1
    {
        Logger logger;
        public Repo_SCP1(Logger _logger)
        {
            logger = _logger;
        }
        public string Add(SCP1 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.SCP1 where e.SocioCode == item.SocioCode && e.DireccionTipo==item.DireccionTipo && e.Calle== item.Calle select e;
                if (t.FirstOrDefault() == null)
                {
                    db.SCP1.Add(item);
                    db.SaveChanges();
                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            return JSONresult;
        }

        public string Get(int direccioncode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP1 where e.DireccionCode == direccioncode select e;
                var result = query.FirstOrDefault();
                if (result != null)
                {
                    if (result.Ventana_Inicio == null || result.Ventana_Inicio == "")
                        result.Ventana_Inicio = "9:00";
                    if (result.Ventana_Termino == null || result.Ventana_Termino == "")
                        result.Ventana_Termino = "20:00";
                    string JSONresult = JsonConvert.SerializeObject(result);
                    //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                    return JSONresult;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Modify(SCP1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP1.Find(item.DireccionCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();

                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public bool Delete(SCP1 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP1.Find(item.DireccionCode);
                if (t != null)
                {
                    db.SCP1.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(int direccioncode)
        {
            using (var db = new cnnDatos())
            {
                var t = db.SCP1.Find(direccioncode);
                if (t != null)
                {
                    db.SCP1.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.SCP1 where e.SocioCode== sociocode select e;
                foreach(var d in t )
                {
                    db.SCP1.Remove(d);
                    db.SaveChanges();
                }
                return true;
            }
        }


        public string List(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.SCP1 where e.SocioCode== sociocode select e;

                var result = query.ToList();
                //List<SCP1> result = new List<SCP1>();
                foreach (var d in result)
                {
                    d.HorarioAtencion = String.Format("{0} - {1}", d.Ventana_Inicio ?? "", d.Ventana_Termino ?? "");
                }
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
