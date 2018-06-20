using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion cuenta financomer
    /// </summary>
    public class FINCTAValidation : ValidationBase<PartidasModel>
    {
        public string EmpresaFinancomer
        {
            get
            {
                return ConfigurationManager.AppSettings["empresaFinancomer"].ToString();
            }
        }

        public FINCTAValidation(PartidasModel context, object objectData) : base(context, objectData)
        {

        }

        public override string Message
        {
            get
            {
                return string.Format(@"Para la empresa de financomer ""{0}""  se requiere concepto de costo con valor  0000000 o vacio", Context.PA_CTA_CONTABLE);
            }
        }

        public override bool Requirement
        {
            get
            {
                if (Context.PA_COD_EMPRESA == this.EmpresaFinancomer)
                {
                    if (Context.PA_CONCEPTO_COSTO == "0000000" || String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                        return true;
                    else
                        return false;
                }
                return true;
            }
        }
    }
}