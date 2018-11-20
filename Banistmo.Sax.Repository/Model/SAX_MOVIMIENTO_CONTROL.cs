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
    
    public partial class SAX_MOVIMIENTO_CONTROL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_MOVIMIENTO_CONTROL()
        {
            this.SAX_MOVIMIENTO_DETALLE = new HashSet<SAX_MOVIMIENTO_DETALLE>();
        }
    
        public int MC_ID_MOVIMIENTO_CONTROL { get; set; }
        public string MC_COD_MOVIMIENTO { get; set; }
        public System.DateTime MC_FECHA_PROCESO { get; set; }
        public string MC_CUENTA_LIMPIEZA { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CC_ID_MONEDA { get; set; }
        public int MC_TOTAL_REGISTRO { get; set; }
        public decimal MC_SALDO_DEBITO { get; set; }
        public decimal MC_SALDO_CREDITO { get; set; }
        public Nullable<decimal> MC_SALDO_TOTAL { get; set; }
        public System.DateTime MC_FECHA_CREACION { get; set; }
        public string MC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> MC_FECHA_MOD { get; set; }
        public string MC_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        public virtual SAX_MONEDA SAX_MONEDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MOVIMIENTO_DETALLE> SAX_MOVIMIENTO_DETALLE { get; set; }
    }
}
