﻿using Banistmo.Sax.Services.Models;
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
        public string  EmpresaFinancomer {
            get {
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
                throw new NotImplementedException();
            }
        }

        public override bool Requirement
        {
            get
            {
                if((Context.PA_COD_EMPRESA == this.EmpresaFinancomer) && (Context.PA_CTA_CONTABLE.StartsWith("51") | Context.PA_CTA_CONTABLE.StartsWith("52")))
                {
                    if (Context.PA_CONCEPTO_COSTO == "0000000")
                        return true;
                    else
                        return false;
                }
                return true;
            }
        }
    }
}