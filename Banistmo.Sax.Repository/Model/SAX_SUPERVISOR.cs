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
    
    public partial class SAX_SUPERVISOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_SUPERVISOR()
        {
            this.SAX_SUPERVISOR_TEMP = new HashSet<SAX_SUPERVISOR_TEMP>();
        }
    
        public int SV_ID_SUPERVISOR { get; set; }
        public string SV_COD_AREA { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string SV_COD_SUPERVISOR { get; set; }
        public string SV_LIMITE_MINIMO { get; set; }
        public string SV_LIMITE_SUPERIOR { get; set; }
        public string SV_TIPO_ACCION { get; set; }
        public string SV_ESTATUS_ACCION { get; set; }
        public int SV_ESTATUS { get; set; }
        public System.DateTime SV_FECHA_CREACION { get; set; }
        public string SV_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> SV_FECHA_MOD { get; set; }
        public string SV_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> SV_FECHA_APROBACION { get; set; }
        public string SV_USUARIO_APROBADOR { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP { get; set; }
    }
}
