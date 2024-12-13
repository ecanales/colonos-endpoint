using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OBDA
    {
        public string Add(OBDA item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.OBDA where e.DocEntry == item.DocEntry && e.BandejaCode==item.BandejaCode select e;
                if (t.FirstOrDefault() == null)
                {
                    db.OBDA.Add(item);
                    db.SaveChanges();
                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
                JSONresult = JsonConvert.SerializeObject(item);
                return JSONresult;
            }
        }

        public string Get(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA where e.DocEntry == docentry select e;
                var result = query.ToList(); // query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Get(string bandeja,int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA where e.DocEntry == docentry && e.BandejaCode==bandeja select e;
                var result = query.FirstOrDefault();
                string JSONresult;
                JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(OBDA item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OBDA.Find(item.DocEntry, item.BandejaCode);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }

                string JSONresult = JsonConvert.SerializeObject(item);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public void Delete(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OBDA.Find(docentry);
                if (t != null)
                {
                    db.OBDA.Remove(t);
                    db.SaveChanges();
                }

            }
        }


        public void Delete(string bandeja, int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = (from e in db.OBDA where e.DocEntry == docentry && e.BandejaCode == bandeja select e).FirstOrDefault(); 
                if (t != null)
                {
                    db.OBDA.Remove(t);
                    db.SaveChanges();
                }

            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA where e.DocEntry== docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string bandejacode, int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA where e.BandejaCode == bandejacode && e.DocEntry== docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
        public string List(string bandejacode, bool estado, bool visible)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA join v in db.OVEN on e.VendedorCode equals v.VendedorCode where e.BandejaCode== bandejacode && e.Estado== estado && e.Visible==visible select new {
                    DocEntry = e.DocEntry,
                    BandejaCode = e.BandejaCode,
                    Estado = e.Estado,
                    FechaIngreso = e.FechaIngreso,
                    UsuarioCodeIngreso = e.UsuarioCodeIngreso,
                    SocioCode = e.SocioCode,
                    RazonSocial = e.RazonSocial,
                    VendedorCode = v.Nombre,
                    FechaAproRech = e.FechaAproRech,
                    MotivoRech = e.MotivoRech,
                    UsuarioCodeAproRech = e.UsuarioCodeAproRech,
                    Visible = e.Visible,
                    Autorizado = e.Autorizado
                    };
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string bandejacode, string sociocode)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA where e.BandejaCode == bandejacode && e.SocioCode== sociocode && e.Estado == true orderby e.FechaIngreso descending select e;
                var result = query.ToList().Take(10);
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(string bandejacode, string sociocode, int top)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OBDA where e.BandejaCode == bandejacode && e.SocioCode == sociocode && e.Estado == true orderby e.FechaAproRech descending select e;
                var result = query.ToList().Take(top);
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
