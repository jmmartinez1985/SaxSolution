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
    
    public partial class SAX_AREA_CENCOSTO
    {
        public int AD_ID_REGISTRO { get; set; }
        public int CA_ID_AREA { get; set; }
        public int EC_ID_REGISTRO { get; set; }
        public int AD_ESTATUS { get; set; }
        public System.DateTime AD_FECHA_CREACION { get; set; }
        public string AD_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> AD_FECHA_MOD { get; set; }
        public string AD_USUARIO_MOD { get; set; }
    
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
        public virtual SAX_EMPRESA_CENTRO SAX_EMPRESA_CENTRO { get; set; }
    }
}
