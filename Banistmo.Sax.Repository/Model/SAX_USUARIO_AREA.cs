//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Banistmo.Sax.Repository.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SAX_USUARIO_AREA
    {
        public string US_COD_USUARIO { get; set; }
        public int CA_COD_AREA { get; set; }
        public int UA_ESTATUS { get; set; }
        public System.DateTime UA_FECHA_CREACION { get; set; }
        public string UA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> UA_FECHA_MOD { get; set; }
        public string UA_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
    }
}
