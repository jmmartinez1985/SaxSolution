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
    
    public partial class SAX_MOVIMIENTO_DETALLE
    {
        public int MD_MOVIMIENTO { get; set; }
        public int MC_ID_MOVIMIENTO_CONTROL { get; set; }
        public System.DateTime MD_FECHA_PROCESO { get; set; }
        public string MD_CUENTA_CONTABLE { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CC_ID_MONEDA { get; set; }
        public decimal MD_IMPORTE { get; set; }
        public string MD_DESCRIPCION { get; set; }
        public System.DateTime MD_FECHA_CREACION { get; set; }
        public string MD_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> MD_FECHA_MOD { get; set; }
        public string MD_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        public virtual SAX_MONEDA SAX_MONEDA { get; set; }
        public virtual SAX_MOVIMIENTO_CONTROL SAX_MOVIMIENTO_CONTROL { get; set; }
    }
}
