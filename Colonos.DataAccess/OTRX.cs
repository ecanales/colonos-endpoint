//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Colonos.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class OTRX
    {
        public int DocEntry { get; set; }
        public Nullable<System.DateTime> DocFecha { get; set; }
        public Nullable<int> DocTipo { get; set; }
        public string DocEstado { get; set; }
        public string BodegaCodeOrigen { get; set; }
        public string BodegaCodeDestino { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
        public Nullable<int> FolioDF { get; set; }
        public Nullable<int> TipoAjuste { get; set; }
        public string Observaciones { get; set; }
    }
}
