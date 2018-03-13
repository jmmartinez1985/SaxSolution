using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class CatalogoModel
    {
        public int CA_ID_CATALOGO { get; set; }
        public string CA_TABLA { get; set; }
        public string CA_DESCRIPCION { get; set; }
        public System.DateTime CA_FECHA_CREACION { get; set; }
        public string CA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CA_FECHA_MOD { get; set; }
        public string CA_USUARIO_MOD { get; set; }

        public List<CatalogoDetalleModel> SAX_CATALOGO_DETALLE { get; set; }
    }
}
