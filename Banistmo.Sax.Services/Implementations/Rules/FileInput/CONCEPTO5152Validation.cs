using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    public class CONCEPTO5152Validation : ValidationBase<PartidasModel>
    {
        private List<EmpresaModel> listaEmpresa;
        public CONCEPTO5152Validation(PartidasModel context, object objectData, List<EmpresaModel> listEmpresa) : base(context, objectData)
        {
            this.listaEmpresa = listEmpresa;
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
                if (string.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                {
                    return string.Format(@"Se requiere concepto de costo para la cuenta ""{0}"" ", Context.PA_CTA_CONTABLE);
                }
                else {
                    return string.Format($"El concepto de costo {Context.PA_CONCEPTO_COSTO} para la cuenta {Context.PA_CTA_CONTABLE} no es válido.");
                }
                
            }
        }

        public override bool Requirement
        {
            get
            {
                if (!String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO.Trim()))
                {
                    if ((Context.PA_COD_EMPRESA != this.EmpresaFinancomer)&&(Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("32"))) {
                        if (string.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                            return false;
                        if (string.IsNullOrEmpty(Context.PA_CTA_CONTABLE)|| string.IsNullOrEmpty(Context.PA_COD_EMPRESA))
                            return false;
                        var empresa = listaEmpresa.First(e => e.CE_COD_EMPRESA == Context.PA_COD_EMPRESA.Trim());
                        string cuentaExcel = Context.PA_CTA_CONTABLE.Substring(0,14);
                        var conceptos = (List<ConceptoCostoModel>)inputObject;
                        
                        int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                        var conceptoCosto = conceptos.Where(cc => cc.CC_NUM_CONCEPTO == Context.PA_CONCEPTO_COSTO.Trim() && cc.CC_CUENTA_MAYOR == cuentaExcel && cc.CC_ESTATUS == activo.ToString() &&cc.CE_ID_EMPRESA== empresa.CE_ID_EMPRESA).ToList();
                        if (conceptoCosto!=null && conceptoCosto.Count==0 ) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                    
                    //ConceptoCostoModel result = conceptos.FirstOrDefault(c => c.CC_NUM_CONCEPTO.Trim() == Context.PA_CONCEPTO_COSTO.Trim());
                    //return result != null ? true : false;
                }
                return true;

            }
        }
    }
}
