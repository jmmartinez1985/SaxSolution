using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//SA: JMMB
namespace Banistmo.Sax.Services.Models
{
    public class UserModel
    {

        [MaxLength(30, ErrorMessage = "Must be Maximum 30 Characters")]
        [Display(Name ="Usuario Codigo")]
        public string US_COD_USUARIO { get; set; }

        public string US_NOMBRE { get; set; }

        public string US_APELLIDO_PATERNO { get; set; }

        public string US_APELLIDO_MATERNO { get; set; }

        public string US_ACTIVO { get; set; }

        public System.DateTime US_FECHA_CREACION { get; set; }

        public string US_USUARIO_CREACION { get; set; }

        public Nullable<System.DateTime> US_FECHA_MOD { get; set; }

        public string US_USUARIO_MOD { get; set; }

        public Nullable<System.DateTime> US_ULTIMO_ACCESO { get; set; }
    }
}
