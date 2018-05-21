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
    /// <summary>
    /// Validacion de empresa
    /// </summary>
    public class CEValidation : ValidationBase<PartidasModel>
    {
        public CEValidation(PartidasModel context, object objectData) : base(context, objectData)
        {

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
                var empresas = (List<EmpresaModel>)inputObject;
                EmpresaModel result = empresas.FirstOrDefault(c => c.CE_COD_EMPRESA == Context.PA_COD_EMPRESA);
                return result != null ? true : false;
            }
        }
    }
}
