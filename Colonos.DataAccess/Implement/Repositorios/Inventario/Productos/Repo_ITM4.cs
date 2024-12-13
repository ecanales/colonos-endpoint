using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_ITM4
    {

        public string Add(ITM4 item)
        {
            using (var db = new cnnDatos())
            {
                db.ITM4.Add(item);
                db.SaveChanges();
                var JSONresult = JsonConvert.SerializeObject(item);
                return JSONresult;
            }
        }

        public string Get(int id)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM4 where e.FamiliaCode==id select e;
                var result = query.FirstOrDefault();
                if (result != null)
                {
                    result.FactorPrecio = result.FactorPrecio ?? 0;
                    result.Volumen = result.Volumen ?? 0;
                    result.DescVolumen = result.DescVolumen ?? 0;
                    result.Margen = result.Margen ?? 0;
                }
                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(ITM4 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM4.Find(item.FamiliaCode);
                if (t!=null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }
                string JSONresult = JsonConvert.SerializeObject(item);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;

            }
        }

        public void Delete(ITM4 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.ITM4.Find(item.FamiliaCode);
                if (t != null)
                {
                    db.ITM4.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.ITM4 orderby e.FamiliaNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string palabras)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spFamilia_Busqueda(palabras) orderby e.FamiliaNombre select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
