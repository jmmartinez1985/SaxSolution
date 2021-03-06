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
    
    public partial class SAX_AREA_OPERATIVA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_AREA_OPERATIVA()
        {
            this.SAX_AREA_CENCOSTO = new HashSet<SAX_AREA_CENCOSTO>();
            this.SAX_CUENTA_CONTABLE = new HashSet<SAX_CUENTA_CONTABLE>();
            this.SAX_SUPERVISOR = new HashSet<SAX_SUPERVISOR>();
            this.SAX_SUPERVISOR_TEMP = new HashSet<SAX_SUPERVISOR_TEMP>();
            this.SAX_USUARIO_AREA = new HashSet<SAX_USUARIO_AREA>();
            this.SAX_COMPROBANTE = new HashSet<SAX_COMPROBANTE>();
            this.SAX_REGISTRO_CONTROL = new HashSet<SAX_REGISTRO_CONTROL>();
            this.SAX_EVENTO = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO_TEMP = new HashSet<SAX_EVENTO_TEMP>();
        }
    
        public int CA_ID_AREA { get; set; }
        public int CA_COD_AREA { get; set; }
        public string CA_NOMBRE { get; set; }
        public int CA_ESTATUS { get; set; }
        public System.DateTime CA_FECHA_CREACION { get; set; }
        public string CA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CA_FECHA_MOD { get; set; }
        public string CA_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_AREA_CENCOSTO> SAX_AREA_CENCOSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CUENTA_CONTABLE> SAX_CUENTA_CONTABLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_AREA> SAX_USUARIO_AREA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_COMPROBANTE> SAX_COMPROBANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_REGISTRO_CONTROL> SAX_REGISTRO_CONTROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP { get; set; }
    }
}
