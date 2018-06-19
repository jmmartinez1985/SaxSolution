using Banistmo.Sax.Common;
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
                return string.Format(@"El código  de moneda ""{0}"" no es válido.", Context.PA_COD_MONEDA);
            }
        }

        public override bool Requirement
        {
            get
            {
               

                string moneda = Context.PA_COD_MONEDA;
                if (string.IsNullOrEmpty(moneda))
                {
                    return false;
                }
                var listaMoneda = (List<MonedaModel>)inputObject;
                int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                MonedaModel result = listaMoneda.FirstOrDefault(c => c.CC_NUM_MONEDA.Trim() == Context.PA_COD_MONEDA.Trim() && c.CC_ESTATUS==activo.ToString());
                if (result == null) {
                    return false;
                }

                else
                {
                    if (Context.PA_COD_EMPRESA != null && Context.PA_COD_EMPRESA.Trim() == "061")
                    {
                        return moneda.Trim() == "0002" ? true : false;
                    }
                    else
                    {
                        return true;
                    }
                }
                
                
            }
        }
    }
}


