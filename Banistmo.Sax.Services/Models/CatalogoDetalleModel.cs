using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class CatalogoDetalleModel
    {
        public int CD_ID_CATALOGO_DETALLE { get; set; }
        public int CA_ID_CATALOGO { get; set; }
        public int CD_TABLA { get; set; }
        public string CD_VALOR { get; set; }
        public int CD_ESTATUS { get; set; }
        public System.DateTime CD_FECHA_CREACION { get; set; }
        public string CD_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CD_FECHA_MOD { get; set; }
        public string CD_USUARIO_MOD { get; set; }

        public CatalogoModel SAX_CATALOGO { get; set; }
    }
}
