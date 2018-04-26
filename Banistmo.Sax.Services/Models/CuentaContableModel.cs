using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class CuentaContableModel
    {
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
        public string CO_COD_AREA { get; set; }
        public string CO_CTA_CONTABLE_CONTRA { get; set; }
        public string CO_COD_AUXILIAR_CONTRA { get; set; }
        public string CO_NUM_AUXILIAR_CONTRA { get; set; }
        public string CO_NOM_CUENTA_CONTRA { get; set; }
        public int CO_ESTATUS { get; set; }
        public System.DateTime CO_FECHA_CREACION { get; set; }
        public string CO_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CO_FECHA_MOD { get; set; }
        public string CO_USUARIO_MOD { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
    }
}
