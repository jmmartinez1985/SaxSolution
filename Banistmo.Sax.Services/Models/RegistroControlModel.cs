using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class RegistroControlModel
    {
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
    }
}
