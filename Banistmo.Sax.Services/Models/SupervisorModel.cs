using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class SupervisorModel
    {
        public int SV_ID_SUPERVISOR { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string SV_COD_SUPERVISOR { get; set; }
        public string SV_LIMITE_MINIMO { get; set; }
        public string SV_LIMITE_SUPERIOR { get; set; }
        public int SV_ESTATUS { get; set; }
        public System.DateTime SV_FECHA_CREACION { get; set; }
        public string SV_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> SV_FECHA_MOD { get; set; }
        public string SV_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> SV_FECHA_APROBACION { get; set; }
        public string SV_USUARIO_APROBADOR { get; set; }
        public int SV_ID_AREA { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP { get; set; }
    }
}
