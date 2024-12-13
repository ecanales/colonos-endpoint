using Colonos.DataAccess;
using Colonos.DataAccess.Repositorios;
using Colonos.Entidades;
using Colonos.Entidades.Defontana;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Manager
{
    public class ManagerPerfiles
    {
        Logger logger;
        cnnDF cnndf;
        public ManagerPerfiles(Logger _logger, cnnDF _cnnDF)
        {
            logger = _logger;
            cnndf = _cnnDF;
        }

        public MensajeReturn ListarUsuario()
        {
            Repo_OUSR repo = new Repo_OUSR();
            var json = repo.List();
            var list= JsonConvert.DeserializeObject<List<User>>(json);

            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.count = list.Count();
            msg.error = false;
            msg.msg = "Listado Usuarios";
            msg.data = list;
            return msg;
        }

        public MensajeReturn ListarUsuario(string idgrupo, string idgrupo2)
        {
            if(idgrupo=="")
            {
                return ListarUsuario();
            }
            Repo_OUSR repo = new Repo_OUSR();
            var json = repo.List();
            var list = JsonConvert.DeserializeObject<List<User>>(json);
            list = list.FindAll(x => x.IdGrupo == idgrupo || x.IdGrupo==idgrupo2);

            MensajeReturn msg = new MensajeReturn();
            msg.statuscode = HttpStatusCode.OK;
            msg.count = list.Count();
            msg.error = false;
            msg.msg = "Listado Usuarios";
            msg.data = list;
            return msg;
        }

        public string AddUsuario(OUSR item)
        {
            Repo_OUSR repo = new Repo_OUSR();


            OUSR us = repo.Add(item);

            User u = new User
            {
                Usuario = us.Usuario,
                Activo = us.Activo,
                IdGrupo = us.IdGrupo,
                Nombre = us.Nombre,
                Rut= us.Rut,
                VendedorDF = us.VendedorDF,
            };
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(u, Formatting.None);

            return JSONresult;

        }

        public string GetUsuario(string IdUsuario)
        {
            var repo = new Repo_OUSR();

            var json = repo.Get(IdUsuario);
            OUSR us = JsonConvert.DeserializeObject<OUSR>(json);
            
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(us, Formatting.None);

            return JSONresult;
        }

        public string GetAccesosGrupo(string idgrupo)
        {
            var repo = new Repo_OGRP();
            List<spSystem_UsAccesos_Result> list = repo.List(idgrupo);
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(list, Formatting.None);

            return JSONresult;

        }

        public string GetConfigMenu()
        {
            var repo = new Repo_OGRP();
            List<spSystem_ConfigMenu_Result> list = repo.ListMenu();
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(list, Formatting.None);

            return JSONresult;

        }

        public string GetGrupoList()
        {
            var repo = new Repo_OGRP();
            List<OGRP> list = repo.List();
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(list, Formatting.None);

            return JSONresult;

        }

        public void AddGrupoAcceso(GRP1 item)
        {
            var repo = new Repo_OGRP();
            repo.AddAcceso(item);

        }

        public string Login(string us, string pw)
        {
            var repo = new Repo_OUSR();

            string json = repo.Login(us, pw);
            return json;
        }

        public string GetUsuario_Login(string IdUsuario)
        {
            var repo = new Repo_OUSR();

            string json = repo.GetUsuario_Login(IdUsuario);
            return json;
        }
    }
}
