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
    
    public partial class SAX_CONCEPTO_COSTO
    {
        public int CC_ID_CONCEPTO_COSTO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string CC_NUM_CONCEPTO { get; set; }
        public string CC_CUENTA_MAYOR { get; set; }
        public string CC_DESCRIPCION { get; set; }
        public string CC_ESTATUS { get; set; }
        public System.DateTime CC_FECHA_CREACION { get; set; }
        public string CC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CC_FECHA_MOD { get; set; }
        public string CC_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
    }
}
