using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class SaldoCuentaValidationModel
    {
        public List<PartidasModel> PartidasList { get; set; }
        public List<CuentaContableModel> CuentasList { get; set; }
    }
}
