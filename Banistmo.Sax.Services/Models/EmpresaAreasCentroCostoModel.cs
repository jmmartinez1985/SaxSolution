using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public partial class EmpresaAreasCentroCostoModel
    {
        public string EmpresaDesc { get; set; }
        public string AreaDesc { get; set; }
        public string CentroCostoDesc { get; set; }
        public int IdArea { get; set; }
        public int IdEmpresa { get; set; }
        public int IdCentroCosto { get; set; }
    }
}
