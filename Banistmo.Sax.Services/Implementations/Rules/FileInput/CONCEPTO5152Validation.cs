﻿using Banistmo.Sax.Common;
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
        private string mensaje;
        public CONCEPTO5152Validation(PartidasModel context, object objectData, List<EmpresaModel> listEmpresa) : base(context, objectData)
        {
            this.listaEmpresa = listEmpresa;
            this.mensaje = "El concepto de costo no es valido o no existe en el sistema";
        }

        public string EmpresaFinancomer
        {
            get
            {
                return ConfigurationManager.AppSettings["empresaFinancomer"].ToString();
            }
        }

        public override string Columna
        {
            get
            {
                return "Concepto de costo";
            }
        }

        public override string Message
        {
            get
            {
                //if (string.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                //{
                //    return string.Format(@"Se requiere concepto de costo para la cuenta ""{0}"".", Context.PA_CTA_CONTABLE);
                //}
                //else if (Context.PA_COD_EMPRESA == this.EmpresaFinancomer && !string.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                //{
                //    return string.Format(@"Para la empresa de financomer ""{0}"", se requiere concepto de costo con valor  0000000 o vacío.", Context.PA_CTA_CONTABLE);
                //}
                //else
                //{
                //    return string.Format($"El concepto de costo {Context.PA_CONCEPTO_COSTO} para la cuenta {Context.PA_CTA_CONTABLE} no es válido.");
                //}

                return mensaje;

            }
        }

        public override bool Requirement
        {
            get
            {
                if (string.IsNullOrEmpty(Context.PA_CTA_CONTABLE)) {
                    return true;
                }
                if (Context.PA_CTA_CONTABLE.Length < 3)
                {
                    return true;
                }
                if (String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO.Trim()))
                {
                    if ((Context.PA_COD_EMPRESA != this.EmpresaFinancomer) && (Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("32")))
                    {
                        if (string.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO)) {
                           mensaje= $"Se requiere concepto de costo para la cuenta {Context.PA_CTA_CONTABLE}" ;
                            return false;
                        }
                           
                        if (string.IsNullOrEmpty(Context.PA_CTA_CONTABLE) || string.IsNullOrEmpty(Context.PA_COD_EMPRESA))
                            return true;
                        var empresa = listaEmpresa.FirstOrDefault(e => e.CE_COD_EMPRESA == Context.PA_COD_EMPRESA.Trim());
                        if (empresa == null) 
                            return true;
                        
                        string cuentaExcel = Context.PA_CTA_CONTABLE.Substring(0, 14);
                        var conceptos = (List<ConceptoCostoModel>)inputObject;

                        int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                        var conceptoCosto = conceptos.Where(cc => cc.CC_NUM_CONCEPTO == Context.PA_CONCEPTO_COSTO.Trim() && cc.CC_CUENTA_MAYOR == cuentaExcel && cc.CC_ESTATUS == activo.ToString() && cc.CE_ID_EMPRESA == empresa.CE_ID_EMPRESA).ToList();
                        if (conceptoCosto != null && conceptoCosto.Count == 0)
                        {
                            mensaje = $"El concepto de costo {Context.PA_CONCEPTO_COSTO} para la cuenta {Context.PA_CTA_CONTABLE} no es válido.";
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (Context.PA_COD_EMPRESA == this.EmpresaFinancomer && !(Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("32")))
                    {
                        if (Context.PA_CONCEPTO_COSTO == "0000000" || String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                            return true;
                        else {
                            mensaje = $"Para la empresa de financomer con cuenta contable {Context.PA_CTA_CONTABLE}, se requiere concepto de costo con valor  0000000 o vacío.";
                            return false;
                        }
                           
                    }

                    else if (Context.PA_COD_EMPRESA == this.EmpresaFinancomer && (Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("32")))
                    {
                        if (Context.PA_CONCEPTO_COSTO == "0000000" || String.IsNullOrEmpty(Context.PA_CONCEPTO_COSTO))
                            return true;
                        else
                        {
                            mensaje = $"Para la empresa de financomer con cuenta contable {Context.PA_CTA_CONTABLE}, se requiere concepto de costo con valor  0000000 o vacío.";
                            return false;
                        }

                    }
                    //else if (Context.PA_COD_EMPRESA != this.EmpresaFinancomer && (Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || Context.PA_CTA_CONTABLE.Trim().Substring(0, 2).Equals("32")))
                    //{
                    //    return true;
                    //}

                    //ConceptoCostoModel result = conceptos.FirstOrDefault(c => c.CC_NUM_CONCEPTO.Trim() == Context.PA_CONCEPTO_COSTO.Trim());
                    //return result != null ? true : false;
                }
               
                return true;

            }
        }
    }
}