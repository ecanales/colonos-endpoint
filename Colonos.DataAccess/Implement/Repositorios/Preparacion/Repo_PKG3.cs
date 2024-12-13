﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.DataAccess
{
    public class Repo_PKG3
    {
        public string Add(PKG3 item)
        {
            string JSONresult = "";
            using (var db = new cnnDatos())
            {
                var t = from e in db.PKG3 where e.DocLinea == item.DocLinea && e.DocEntry == item.DocEntry select e;
                if (t.FirstOrDefault() == null)
                {
                    db.PKG3.Add(item);
                    db.SaveChanges();

                    JSONresult = JsonConvert.SerializeObject(item);
                    return JSONresult;
                }
            }
            JSONresult = JsonConvert.SerializeObject(item);
            //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
            return JSONresult;
        }

        public string Get(int doclinea)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG3 where e.DocLinea == doclinea select e;
                var result = query.FirstOrDefault();

                string JSONresult = JsonConvert.SerializeObject(result);
                //JSONresult = JSONresult.Substring(1, JSONresult.Length - 2);
                return JSONresult;
            }
        }

        public string Modify(PKG3 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PKG3.Find(item.DocEntry, item.DocLinea);
                if (t != null)
                {
                    db.Entry(t).CurrentValues.SetValues(item);
                    db.SaveChanges();
                }
                var result = item;
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public bool Delete(PKG3 item)
        {
            using (var db = new cnnDatos())
            {
                var t = db.PKG3.Find(item.DocEntry, item.DocLinea);
                if (t != null)
                {
                    db.PKG3.Remove(t);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool Delete(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var t = from e in db.PKG3 where e.DocEntry == docentry select e;
                foreach (var l in t)
                {
                    db.PKG3.Remove(l);
                    db.SaveChanges();
                }
                return true;
            }
        }

        public string List()
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG3 select e;

                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }

        public string List(int docentry)
        {
            using (var db = new cnnDatos())
            {
                var query = from e in db.PKG3 where e.DocEntry == docentry select e;
                var result = query.ToList();
                string JSONresult = JsonConvert.SerializeObject(result);
                return JSONresult;
            }
        }
    }
}
