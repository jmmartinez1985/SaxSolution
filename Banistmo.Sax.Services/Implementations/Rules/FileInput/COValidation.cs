using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Implementations.Business;
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
    /// Validacion de cuenta contable
    /// </summary>
    public class COValidation : ValidationBase<PartidasModel>
    {

        private string mensaje;
        public COValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
            mensaje = "La cuenta contable  no existe o esta inactiva en el sistema.";
        }

        public override string Columna
        {
            get
            {
                return "Cuenta contable";
            }
        }
        public override string Message
        {
            get
            {
                return mensaje;
            }
        }

        public override bool Requirement
        {
            get
            {
                if (String.IsNullOrEmpty(Context.PA_CTA_CONTABLE)) {
                    mensaje= $"La cuenta contable no puede estar vacia";
                    return true;
                }
                var cuentas = (List<CuentaContableModel>)inputObject;
                int  activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                CuentaContableModel result = cuentas.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim().ToUpper() + c.CO_COD_AUXILIAR.Trim().ToUpper() + c.CO_NUM_AUXILIAR.Trim().ToUpper()) == Context.PA_CTA_CONTABLE.Trim().ToUpper() && c.CO_ESTATUS== activo);
                mensaje = $"La cuenta contable {Context.PA_CTA_CONTABLE} no existe o esta inactiva en el sistema.";
                return result != null ? true : false;
            }
        }
    }
}
