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
    
    public partial class SAX_CATALOGO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_CATALOGO()
        {
            this.SAX_CATALOGO_DETALLE = new HashSet<SAX_CATALOGO_DETALLE>();
        }
    
        public int CA_ID_CATALOGO { get; set; }
        public string CA_TABLA { get; set; }
        public string CA_DESCRIPCION { get; set; }
        public System.DateTime CA_FECHA_CREACION { get; set; }
        public string CA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CA_FECHA_MOD { get; set; }
        public string CA_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CATALOGO_DETALLE> SAX_CATALOGO_DETALLE { get; set; }
    }
}
