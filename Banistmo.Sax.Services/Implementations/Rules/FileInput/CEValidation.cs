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
    /// <summary>
    /// Validacion de empresa
    /// </summary>
    public class CEValidation : ValidationBase<PartidasModel>
    {
        private string mensaje;
        
        public CEValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
            mensaje= "La empresa no  existe o esta inactiva en el sistema.";
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
                return "Empresa";
            }
        }

        public override bool Requirement
        {
            get
            {
                var empresas = (List<EmpresaModel>)inputObject;
                int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                if (string.IsNullOrEmpty(Context.PA_COD_EMPRESA))
                {
                    mensaje = "El códio de empresa es requerido.";
                    return false;
                }
                EmpresaModel result = empresas.FirstOrDefault(c => c.CE_COD_EMPRESA.Trim() == Context.PA_COD_EMPRESA.Trim() && c.CE_ESTATUS== activo.ToString());
                return result != null ? true : false;
            }
        }
    }
}
