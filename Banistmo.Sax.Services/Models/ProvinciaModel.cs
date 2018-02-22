using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ProvinciaModel
    {
        public int PR_Id { get; set; }
        public int PA_Id { get; set; }
        public string PR_Descripcion { get; set; }

        public virtual PaisModel PA_PAISES { get; set; }
    }
}
