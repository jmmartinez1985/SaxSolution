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
    class COValidation : ValidationBase<PartidasModel>
    {
        private readonly ICuentaContableService contableService;
               
        public COValidation(PartidasModel context, ICuentaContableService cuenta_contable) : base(context)
        {
            contableService = cuenta_contable;
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
                CuentaContableModel result = contableService.GetSingle(c => c.CO_NOM_CUENTA == Context.PA_CTA_CONTABLE);
                return result != null ? true : false;
            }
        }
    }
}
