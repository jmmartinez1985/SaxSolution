using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de moneda
    /// </summary>
    public class CMValidations: ValidationBase<PartidasModel>
    {
        public string codigoMoneda;
        public CMValidations(PartidasModel context, object objectData) : base(context, objectData)
        {
            this.codigoMoneda = "0001";
        }
        public override string Message
        {
            get
            {
                return string.Format(@"El código  de moneda ""{0}"" no es válido.", Context.PA_COD_MONEDA);
            }
        }

        public override bool Requirement
        {
            get
            {
                return this.codigoMoneda == Context.PA_COD_MONEDA;
            }
        }
    }
}
