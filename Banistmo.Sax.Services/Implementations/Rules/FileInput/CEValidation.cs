using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    class CEValidation : ValidationBase<PartidasModel>
    {
        private readonly IEmpresaService service_empresa;
        public CEValidation(PartidasModel context, IEmpresaService parm_empresa) : base(context)
        {
            this.service_empresa = parm_empresa;
        }
        public override string Message
        {
            get
            {
                return string.Format(@"El código  de empresa ""{0}"" no es válido.", Context.PA_COD_EMPRESA);
            }
        }

        public override bool Requirement
        {
            get
            {
                EmpresaModel result = service_empresa.GetSingle(c => c.CE_COD_EMPRESA == Context.PA_COD_EMPRESA);
                return result != null ? true : false;
            }
        }
    }
}
