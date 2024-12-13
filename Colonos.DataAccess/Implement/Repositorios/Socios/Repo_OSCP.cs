using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OSCP
    {
        Logger logger;
        public Repo_OSCP(Logger _logger)
        {
            logger = _logger;
        }
        public string Add(OSCP item)
        {
            string JSONresult = "";
            item.SocioCode = String.Format("{0}{1}", item.Rut, item.SocioTipo.ToString());
            
            using (var db = new cnnDatos())
            {
                var t = from e in db.OSCP where e.SocioCode == item.SocioCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OSCP.Add(item);
                    db.SaveChanges();
                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(string id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OSCP where e.SocioCode == id select e;
                var result = query.FirstOrDefault();
                if (result != null)
                {
                    string JSONresult = JsonConvert.SerializeObject(result);
                    //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                    return JSONresult;
                }

                return "";
            }
        }

        public string Modify(OSCP item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OSCP.Find(item.SocioCode);
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

        public bool Delete(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OSCP.Find(sociocode);
                if (t != null)
                {
                    db.OSCP.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OSCP select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string palabras, string tipo,string usuario)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spSocio_Busqueda(palabras, tipo, usuario) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        //----------------------------------------------------
        public string TopFamiliaCliente(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spTopCliente(sociocode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string TopVentasCliente(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spUltimasVentasCliente(sociocode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string TopFamiliaRubro(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spTopRubro(sociocode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string TopUltimosPrecios(string sociocode, string familiacode)
        {
            if (familiacode.Trim().Length == 0)
                familiacode = "0";
            using (var db = new cnnDatos())
            {
                var query = from e in db.spUltimosPreciosClienteFamilia(sociocode, Convert.ToInt32(familiacode)) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string CuentaCorriente(string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var sociocodedf = (from e in db.OSCP where e.SocioCode == sociocode select e).FirstOrDefault();
                if (sociocodedf != null)
                {
                    var query = from e in db.SCP11 where e.SocioCode == sociocodedf.clientFileDF select e;

                    var result = query.ToList();
                    string JSONresult = JsonConvert.SerializeObject(result);
                    //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                    return JSONresult;
                }
                else
                {
                    return  "";
                }
            }
        }

        public string Ventas12Meses(string sociocode)
        {

            using (var db = new cnnDatos())
            {
                var query = from e in db.spSocio_Ventas12meses(sociocode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
    }
}
