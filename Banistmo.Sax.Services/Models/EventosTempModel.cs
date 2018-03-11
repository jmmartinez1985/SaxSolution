using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class EventosTempModel
    {
        public int EV_COD_EVENTO_TEMP { get; set; }
        public int EV_COD_EVENTO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CA_COD_AREA { get; set; }
        public string EV_DESCRIPCION_EVENTO { get; set; }
        public string EV_CUENTA_DEBITO { get; set; }
        public string EV_COD_AUXILIARD { get; set; }
        public string EV_NUM_AUXILIARD { get; set; }
        public string EV_CUENTA_CREDITO { get; set; }
        public string EV_COD_AUXILIARC { get; set; }
        public string EV_NUM_AUXILIARC { get; set; }
        public string EV_REFERENCIA { get; set; }
        public string EV_TIPO_ACCION { get; set; }
        public string EV_ESTATUS_ACCION { get; set; }
        public int EV_ESTATUS { get; set; }
        public DateTime EV_FECHA_CREACION { get; set; }
        public string EV_USUARIO_CREACION { get; set; }
        public DateTime EV_FECHA_MOD { get; set; }
        public string EV_USUARIO_MOD { get; set; }
        public DateTime EV_FECHA_APROBACION { get; set; }
        public string EV_USUARIO_APROBADOR { get; set; }
    
        public virtual SAX_EVENTO SAX_EVENTO { get; set; }
    }
}
