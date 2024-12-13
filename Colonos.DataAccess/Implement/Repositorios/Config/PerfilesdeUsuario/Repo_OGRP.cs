using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess.Repositorios
{
    public class Repo_OGRP
    {
        public OGRP Add(OGRP item)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OGRP where e.Nombre == item.Nombre select e;
                if (query.FirstOrDefault() != null)
                {
                    item.IdGrupo = query.FirstOrDefault().IdGrupo;
                    return this.Modify(item);
                }
                db.OGRP.Add(item);
                db.SaveChanges();

                return item;

            }
        }

        public void AddAcceso(GRP1 item)
        {
            using (var db = new cnnDatos())
            {
                db.spSystem_GruposAddAcceso(item.FK_MenuMain, item.FK_MenuSubA, item.FK_MenuSubB, item.FK_Grupo, item.Acceso);
                db.SaveChanges();

            }
        }

        public OGRP Get(string idgrupo)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OGRP where e.IdGrupo == idgrupo select e;
                return query.FirstOrDefault();

            }
        }

        public OGRP Modify(OGRP item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.OGRP.Find(item.IdGrupo);

                if (!t.Equals(item))
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }


                return item;
            }
        }

        public List<OGRP> List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.OGRP orderby e.Nombre select e;
                return query.ToList();


            }
        }

        public List<spSystem_UsAccesos_Result> List(string idgrupo)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spSystem_UsAccesos(idgrupo) select e;
                return query.ToList();


            }
        }

        public List<spSystem_ConfigMenu_Result> ListMenu()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.spSystem_ConfigMenu() select e;
                return query.ToList();


            }
        }
    }
}
