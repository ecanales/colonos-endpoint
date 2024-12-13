using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OITM
    {
        Logger logger;
        public Repo_OITM(Logger _logger)
        {
            logger = _logger;
        }
        public string Add(OITM item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OITM where e.ProdCode == item.ProdCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OITM.Add(item);
                    db.SaveChanges();
                    //crear registro en todas las bodegas
                    Repo_OITB repo = new Repo_OITB();
                    Repo_OBOD repobod = new Repo_OBOD();
                    var json = repobod.List();
                    var list=JsonConvert.DeserializeObject<List<OBOD>>(json);

                    foreach (var b in list)
                    {
                        repo.Add(new OITB { ProdCode = item.ProdCode, BodegaCode = b.BodegaCode, Asignado = 0, Costo = 0, Stock = 0,Ordenado=0 });
                    }    

                    JSONresult = JsonConvert.SerializeObject(item);
                    //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
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
                var query = from e in db.OITM where e.ProdCode == id select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OITM item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OITM.Find(item.ProdCode);
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

        public bool Delete(string prodcode)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OITM.Find(prodcode);
                if (t != null)
                {
                    db.OITM.Remove(t);
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
                var query = from e in db.OITM select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        //public string List(int activo)
        //{
        //    using (var db = new cnnDatos())
        //    {
        //        var query = from e in db.OITM select e;

        //        var result = query.ToList();
        //        if(activo==1)
        //        {
        //            result = result.FindAll(x => x.Activo == "S");
        //        }
        //        string JSONresult = JsonConvert.SerializeObject(result);
        //        return JSONresult;
        //    }
        //}

        public string List(string activo, string todos)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OITM where e.Activo== activo select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string desglose)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OITM where e.EsDesglose == desglose select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string activo,bool maeprod)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spOITM_Listar(activo) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string ListMaestro(string palabras, string solorecetas)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spProducto_Busqueda_Maestro(palabras, solorecetas) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string List(string palabras, string solorecetas, string bodegacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spProducto_Busqueda(palabras, solorecetas, bodegacode) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public void ActivarProductosConStock()
        {
            using (var db = new cnnDatos())
            {
                db.Database.ExecuteSqlCommand("exec dbo.spProducto_Activar_Prod_Con_Stock");
            }
        }

        public string ListStock(string palabras)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spProducto_ConsultaStock(palabras) select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string GetCorrelativoFamilia(int familiacode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spProducto_Correlativo(familiacode) select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }
    }
}
