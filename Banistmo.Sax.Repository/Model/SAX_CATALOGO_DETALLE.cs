//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Banistmo.Sax.Repository.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SAX_CATALOGO_DETALLE
    {
        public int CD_ID_CATALOGO_DETALLE { get; set; }
        public int CA_ID_CATALOGO { get; set; }
        public int CD_TABLA { get; set; }
        public string CD_VALOR { get; set; }
        public int CD_ESTATUS { get; set; }
        public System.DateTime CD_FECHA_CREACION { get; set; }
        public string CD_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CD_FECHA_MOD { get; set; }
        public string CD_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_CATALOGO SAX_CATALOGO { get; set; }
    }
}
