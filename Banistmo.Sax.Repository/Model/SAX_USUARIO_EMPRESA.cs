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
    
    public partial class SAX_USUARIO_EMPRESA
    {
        public int UE_ID_USUARIO_EMPRESA { get; set; }
        public string US_ID_USUARIO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int UE_ESTATUS { get; set; }
        public System.DateTime UE_FECHA_CREACION { get; set; }
        public string UE_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> UE_FECHA_MOD { get; set; }
        public string UE_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
    }
}
