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
    
    public partial class SAX_CUENTA_CONTABLE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_CUENTA_CONTABLE()
        {
            this.SAX_SALDO_CONTABLE = new HashSet<SAX_SALDO_CONTABLE>();
            this.SAX_EVENTO = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO1 = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO_TEMP = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_EVENTO_TEMP1 = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_SALDO_NOCONCILIABLE = new HashSet<SAX_SALDO_NOCONCILIABLE>();
        }
    
        public int CO_ID_CUENTA_CONTABLE { get; set; }
        public int CO_INSTITUCION { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string CO_CUENTA_CONTABLE { get; set; }
        public string CO_COD_AUXILIAR { get; set; }
        public string CO_NUM_AUXILIAR { get; set; }
        public string CO_NOM_CUENTA { get; set; }
        public string CO_NOM_AUXILIAR { get; set; }
        public string CO_COD_CONCILIA { get; set; }
        public string CO_COD_NATURALEZA { get; set; }
        public string CO_CTA_CONTABLE_CONTRA { get; set; }
        public string CO_COD_AUXILIAR_CONTRA { get; set; }
        public string CO_NUM_AUXILIAR_CONTRA { get; set; }
        public string CO_NOM_CUENTA_CONTRA { get; set; }
        public int CO_ESTATUS { get; set; }
        public System.DateTime CO_FECHA_CREACION { get; set; }
        public string CO_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CO_FECHA_MOD { get; set; }
        public string CO_USUARIO_MOD { get; set; }
        public Nullable<int> ca_id_area { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SALDO_CONTABLE> SAX_SALDO_CONTABLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SALDO_NOCONCILIABLE> SAX_SALDO_NOCONCILIABLE { get; set; }
    }
}
