using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    class MONEDAValidation : ValidationBase<PartidasModel>
    {

        public MONEDAValidation(PartidasModel context, object objectData) : base(context, objectData)
        {

        }
        public override string Message
        {
            get
            {
                return string.Format(@"El código  de modena ""{0}"" no es válido.", Context.PA_COD_MONEDA);
            }
        }

        public override bool Requirement
        {
            get
            {
                var empresas = (List<EmpresaModel>)inputObject;
                string moneda = Context.PA_COD_MONEDA;
                if (string.IsNullOrEmpty(moneda))
                {
                    return false;
                }
                else {
                    if (Context.PA_COD_EMPRESA != null && Context.PA_COD_EMPRESA.Trim() == "061")
                    {
                        return moneda == "002" ? true : false;
                    }
                    else if (moneda == "001")
                    {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                
                
            }
        }
    }
}


