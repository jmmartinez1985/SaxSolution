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
    
    public partial class SAX_USUARIO_ROL
    {
        public int UR_ID_USUARIO_ROL { get; set; }
        public string US_ID_USUARIO { get; set; }
        public int RL_ID_ROL { get; set; }
        public int UR_ESTATUS { get; set; }
        public System.DateTime UR_FECHA_CREACION { get; set; }
        public string UR_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> UR_FECHA_MOD { get; set; }
        public string UR_USUARIO_MOD { get; set; }
    
        public virtual SAX_ROLES SAX_ROLES { get; set; }
        public virtual SAX_USUARIO SAX_USUARIO { get; set; }
    }
}
