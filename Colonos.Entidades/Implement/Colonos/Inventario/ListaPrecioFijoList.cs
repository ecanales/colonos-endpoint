using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colonos.Entidades
{
    public class ListaPrecioFijoList
    {
        public string ProdCode {get;set;}
        public DateTime Desde {get;set;}
        public DateTime Hasta   {get;set;}
        public decimal? Precio   {get;set;}
        public string ListaCode  {get;set;}
        public string ProdNombre  {get;set;}
        public string FamiliaNombre  {get;set;}
        public string OrigenNombre  {get;set;}
        public decimal? Costo {get;set;}
        public decimal? Margen {get;set;}
        public decimal? PrecioNormal { get; set; }

    }
}
