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
    
    public partial class SAX_SUPERVISOR_TEMP
    {
        public int SV_ID_SUPERVISOR_TEMP { get; set; }
        public int SV_ID_SUPERVISOR { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string SV_COD_SUPERVISOR { get; set; }
        public string SV_LIMITE_MINIMO { get; set; }
        public string SV_LIMITE_SUPERIOR { get; set; }
        public int SV_ESTATUS { get; set; }
        public System.DateTime SV_FECHA_CREACION { get; set; }
        public string SV_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> SV_FECHA_MOD { get; set; }
        public string SV_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> SV_FECHA_APROBACION { get; set; }
        public string SV_USUARIO_APROBADOR { get; set; }
        public Nullable<int> SV_ID_AREA { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
        public virtual AspNetUsers AspNetUsers3 { get; set; }
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        public virtual SAX_SUPERVISOR SAX_SUPERVISOR { get; set; }
    }
}
