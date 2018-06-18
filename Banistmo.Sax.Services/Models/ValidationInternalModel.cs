using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ValidationInternalModel
    {

    }

    public class MonedaValidationModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

    }

    public class EmpresaValidationModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

    }

    public class EmpresaMonedaValidationModel
    {
        public string CodigoMoneda { get; set; }
        public string DescripcionMoneda { get; set; }

        public string CodigoEmpresa { get; set; }
        public string DescripcionEmpresa { get; set; }

    }

}
