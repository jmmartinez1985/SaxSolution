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
    
    public partial class SAX_REGISTRO_CONTROL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_REGISTRO_CONTROL()
        {
            this.SAX_PARTIDAS = new HashSet<SAX_PARTIDAS>();
        }
    
        public int RC_REGISTRO_CONTROL { get; set; }
        public string RC_COD_PARTIDA { get; set; }
        public string RC_COD_USUARIO { get; set; }
        public string RC_COD_OPERACION { get; set; }
        public string RC_ARCHIVO { get; set; }
        public System.DateTime RC_FECHA_PROCESO { get; set; }
        public string RC_COD_AREA { get; set; }
        public int RC_TOTAL_REGISTRO { get; set; }
        public decimal RC_TOTAL_DEBITO { get; set; }
        public decimal RC_TOTAL_CREDITO { get; set; }
        public decimal RC_TOTAL { get; set; }
        public string RC_ESTATUS_LOTE { get; set; }
        public string RC_COD_EVENTO { get; set; }
        public System.DateTime RC_FECHA_CREACION { get; set; }
        public string RC_USUARIO_CREACION { get; set; }
        public System.DateTime RC_FECHA_APROBACION { get; set; }
        public string RC_USUARIO_APROBADOR { get; set; }
        public System.DateTime RC_FECHA_MOD { get; set; }
        public string RC_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARTIDAS> SAX_PARTIDAS { get; set; }
    }
}
