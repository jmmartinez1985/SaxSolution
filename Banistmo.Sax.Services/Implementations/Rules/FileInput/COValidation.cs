using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de cuenta contable
    /// </summary>
    public class COValidation : ValidationBase<PartidasModel>
    {
        public COValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format(@"La cuenta contable ""{0}"" no es válida.", Context.PA_CTA_CONTABLE);
            }
        }

        public override bool Requirement
        {
            get
            {
                var cuentas = (List<CuentaContableModel>)inputObject;
                CuentaContableModel result = cuentas.FirstOrDefault(c => (c.CO_NOM_CUENTA + c.CO_COD_AUXILIAR  + c.CO_NUM_AUXILIAR) == Context.PA_CTA_CONTABLE);
                return result != null ? true : false;
            }
        }
    }
}
