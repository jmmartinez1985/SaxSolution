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
    /// <summary>
    /// Validacion de centro de costo
    /// </summary>
    public class CCValidations : ValidationBase<PartidasModel>
    {
        public CCValidations(PartidasModel context, object objectData) : base(context, objectData)
        {
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
                var centrocosto = (List<CentroCostoModel>)inputObject;
                var result = centrocosto.FirstOrDefault(c => c.CC_CENTRO_COSTO == Context.PA_CENTRO_COSTO);
                return result != null ? true : false;
            }
        }
    }
}
