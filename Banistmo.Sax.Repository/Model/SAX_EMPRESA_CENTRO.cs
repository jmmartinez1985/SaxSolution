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
    
    public partial class SAX_EMPRESA_CENTRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SAX_EMPRESA_CENTRO()
        {
            this.SAX_AREA_CENCOSTO = new HashSet<SAX_AREA_CENCOSTO>();
        }
    
        public int EC_ID_REGISTRO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CC_ID_CENTRO_COSTO { get; set; }
        public int EC_ESTATUS { get; set; }
        public System.DateTime EC_FECHA_CREACION { get; set; }
        public string EC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> EC_FECHA_MOD { get; set; }
        public string EC_USUARIO_MOD { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_AREA_CENCOSTO> SAX_AREA_CENCOSTO { get; set; }
        public virtual SAX_CENTRO_COSTO SAX_CENTRO_COSTO { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
    }
}
