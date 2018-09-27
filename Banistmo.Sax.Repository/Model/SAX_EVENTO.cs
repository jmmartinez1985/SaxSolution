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
    
    public partial class SAX_EVENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_EVENTO()
        {
            this.SAX_EVENTO_TEMP = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_REGISTRO_CONTROL = new HashSet<SAX_REGISTRO_CONTROL>();
        }
    
        public int EV_COD_EVENTO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int EV_ID_AREA { get; set; }
        public string EV_DESCRIPCION_EVENTO { get; set; }
        public int EV_CUENTA_DEBITO { get; set; }
        public int EV_CUENTA_CREDITO { get; set; }
        public string EV_REFERENCIA_DEBITO { get; set; }
        public string EV_REFERENCIA_CREDITO { get; set; }
        public string EV_ESTATUS_ACCION { get; set; }
        public int EV_ESTATUS { get; set; }
        public System.DateTime EV_FECHA_CREACION { get; set; }
        public string EV_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> EV_FECHA_MOD { get; set; }
        public string EV_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> EV_FECHA_APROBACION { get; set; }
        public string EV_USUARIO_APROBADOR { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
        public virtual SAX_CUENTA_CONTABLE SAX_CUENTA_CONTABLE { get; set; }
        public virtual SAX_CUENTA_CONTABLE SAX_CUENTA_CONTABLE1 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_REGISTRO_CONTROL> SAX_REGISTRO_CONTROL { get; set; }
    }
}
