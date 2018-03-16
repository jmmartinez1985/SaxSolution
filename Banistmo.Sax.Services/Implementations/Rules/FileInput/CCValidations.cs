using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    class CCValidations : ValidationBase<PartidasModel>
    {
        private readonly ICentroCosto service_centro_costo;
        public CCValidations(PartidasModel context, ICentroCosto parm_centro_costo) : base(context)
        {
            this.service_centro_costo = parm_centro_costo;
        }
        public override string Message
        {
            get
            {
                return string.Format(@"El centro de costo ""{0}"" no es válido.", Context.PA_COD_EMPRESA);
            }
        }

        public override bool Requirement
        {
            get
            {
                SAX_CENTRO_COSTO result = service_centro_costo.GetSingle(c => c.CC_CENTRO_COSTO == Context.PA_CENTRO_COSTO);
                return result != null ? true : false;
            }
        }
    }
}
