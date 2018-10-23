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
        private string mensaje;
        public MONEDAValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
            mensaje = "El código  de moneda no existe o está inactiva.";
        }
        public override string Message
        {
            get
            {
                return mensaje;
            }
        }

        public override string Columna
        {
            get
            {
                return "Código de moneda";
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
                    mensaje =$"El código  de moneda {Context.PA_COD_MONEDA} no existe o está inactiva.";
                    return false;
                }

                else
                {
                    if (Context.PA_COD_EMPRESA != null && Context.PA_COD_EMPRESA.Trim() == "061")
                    {
                        mensaje = $"La empresa 061  solo admite moneda 0002";
                        return moneda.Trim() == "0002" ? true : false;
                    }
                    else
                    {
                        if (Context.PA_COD_EMPRESA != null && Context.PA_COD_EMPRESA.Trim() != "061" && moneda.Trim() == "0002")
                        {
                            mensaje = $"La moneda 0002 es de uso exclusivo de la empresa Financomer.";
                            return false;
                        }
                        else {
                            return true;
                        }
                        
                    }
                }
                
                
            }
        }
    }
}


