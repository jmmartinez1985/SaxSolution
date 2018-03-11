using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Banistmo.Sax.Services.Models
{
    public class EventosModel
    {
        [Required]       
        public int EV_COD_EVENTO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CA_COD_AREA { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string EV_DESCRIPCION_EVENTO { get; set; }

        [StringLength(14, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_CUENTA_DEBITO { get; set; }

        [StringLength(3, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_COD_AUXILIARD { get; set; }

        [StringLength(13, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_NUM_AUXILIARD { get; set; }

        [StringLength(14, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_CUENTA_CREDITO { get; set; }

        [StringLength(3, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_COD_AUXILIARC { get; set; }

        [StringLength(13, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string  EV_NUM_AUXILIARC { get; set; }

        [StringLength(1, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_REFERENCIA { get; set; }

        [StringLength(15, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_TIPO_ACCION { get; set; }

        [StringLength(15, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_ESTATUS_ACCION { get; set; }
        public int EV_ESTATUS { get; set; }
        public DateTime EV_FECHA_CREACION { get; set; }

        [StringLength(128, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_USUARIO_CREACION { get; set; }
        public DateTime EV_FECHA_MOD { get; set; }

        [StringLength(128, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_USUARIO_MOD { get; set; }
        public DateTime EV_FECHA_APROBACION { get; set; }

        [StringLength(128, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string EV_USUARIO_APROBADOR { get; set; }
    }
}
