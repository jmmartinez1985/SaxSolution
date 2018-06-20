using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de concepto de costo
    /// </summary>
    public class CONCEPCOSValidation : ValidationBase<PartidasModel>
    {
        public CONCEPCOSValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }

        public string EmpresaFinancomer
        {
            get
            {
                return ConfigurationManager.AppSettings["empresaFinancomer"].ToString();
            }
        }
        public override string Message
        {
            get
            {
                if (!String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                {
                    return string.Format(@"La cuenta contable no puede tener concepto de costo.", Context.PA_CTA_CONTABLE);
                }
                else {
                    return string.Format(@"El concepto de costo ""{0}"" no es válido.", Context.PA_CONCEPTO_COSTO);
                }
                
            }
        }

        public override bool Requirement
        {
            get
            {
                if ((Context.PA_COD_EMPRESA != this.EmpresaFinancomer) && (Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("32"))) {
                    return false;
                }

                if (!String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO.Trim()))
                {
                    var conceptos = (List<ConceptoCostoModel>)inputObject;
                    int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                    ConceptoCostoModel result = conceptos.FirstOrDefault(c => c.CC_NUM_CONCEPTO.Trim() == Context.PA_CONCEPTO_COSTO.Trim() && c.CC_ESTATUS== activo.ToString());
                    return result != null ? true : false;
                }
                return true;

            }
        }
    }
}
