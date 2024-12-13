using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    //Tabla OSCP
    public class Socio
    {
        
        public string SocioCode	{get;set;}
        public string Rut {get;set;}
        public string RazonSocial {get;set;}
        public string NombreFantasia {get;set;}
        public string SocioTipo {get;set;}
        public string EmailSII {get;set;}
        public string Giro {get;set;}
        public int? RubroCode {get;set;}
        public string RubroName {get;set;}
        public int? CondicionCode {get;set;}
        public string CondicionName {get;set;}
        public string VendedorCode {get;set;}
        public string MatrizSocio {get;set;}
        public decimal? CreditoAutorizado {get;set;}
        public decimal? CreditoUtiliado {get;set;}
        public string EstadoOperativo {get;set;}
        public List<Direccion> Direcciones { get; set; }
        public List<Contacto> Contactos { get; set; }
        public List<Archivo> Archivos { get; set; }
        public string clientFileDF { get; set; }
        public decimal? CreditoMaximo { get; set; }
        public string CondicionDF { get; set; }
        public string Activo { get; set; }
        public List<HistorialModificaciones> Historial { get; set; }
    }
}
