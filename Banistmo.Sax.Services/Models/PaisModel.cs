using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class PaisModel
    {
        public int PA_Id { get; set; }
        public string PA_Descripcion { get; set; }
        public virtual ICollection<ProvinciaModel> PR_PROVINCIA { get; set; }
    }
}
