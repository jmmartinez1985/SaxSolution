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
    
    public partial class SAX_AREA_OPERATIVA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_AREA_OPERATIVA()
        {
            this.SAX_AREA_CENCOSTO = new HashSet<SAX_AREA_CENCOSTO>();
            this.SAX_EVENTO = new HashSet<SAX_EVENTO>();
            this.SAX_SUPERVISOR = new HashSet<SAX_SUPERVISOR>();
            this.SAX_USUARIO_AREA = new HashSet<SAX_USUARIO_AREA>();
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
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_AREA> SAX_USUARIO_AREA { get; set; }
    }
}
