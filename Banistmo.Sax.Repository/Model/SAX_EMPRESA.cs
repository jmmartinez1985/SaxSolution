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
    
    public partial class SAX_EMPRESA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_EMPRESA()
        {
            this.SAX_CONCEPTO_COSTO = new HashSet<SAX_CONCEPTO_COSTO>();
            this.SAX_CUENTA_CONTABLE = new HashSet<SAX_CUENTA_CONTABLE>();
            this.SAX_EMPRESA_CENTRO = new HashSet<SAX_EMPRESA_CENTRO>();
            this.SAX_SUPERVISOR = new HashSet<SAX_SUPERVISOR>();
            this.SAX_SUPERVISOR_TEMP = new HashSet<SAX_SUPERVISOR_TEMP>();
            this.SAX_USUARIO_EMPRESA = new HashSet<SAX_USUARIO_EMPRESA>();
            this.SAX_EVENTO = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO_TEMP = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_MOVIMIENTO_CONTROL = new HashSet<SAX_MOVIMIENTO_CONTROL>();
            this.SAX_MOVIMIENTO_DETALLE = new HashSet<SAX_MOVIMIENTO_DETALLE>();
            this.SAX_SALDO_NOCONCILIABLE = new HashSet<SAX_SALDO_NOCONCILIABLE>();
        }
    
        public int CE_ID_EMPRESA { get; set; }
        public string CE_COD_EMPRESA { get; set; }
        public string CE_NOMBRE { get; set; }
        public string CE_ESTATUS { get; set; }
        public System.DateTime CE_FECHA_CREACION { get; set; }
        public string CE_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CE_FECHA_MOD { get; set; }
        public string CE_USUARIO_MOD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CONCEPTO_COSTO> SAX_CONCEPTO_COSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CUENTA_CONTABLE> SAX_CUENTA_CONTABLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EMPRESA_CENTRO> SAX_EMPRESA_CENTRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_EMPRESA> SAX_USUARIO_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MOVIMIENTO_CONTROL> SAX_MOVIMIENTO_CONTROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MOVIMIENTO_DETALLE> SAX_MOVIMIENTO_DETALLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SALDO_NOCONCILIABLE> SAX_SALDO_NOCONCILIABLE { get; set; }
    }
}
