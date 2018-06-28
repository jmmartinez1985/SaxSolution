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
        public CEValidation(PartidasModel context, object objectData) : base(context, objectData)
        {

        }
        public override string Message
        {
            get
            {
                return string.Format(@"El código  de empresa ""{0}"" no es válido.", Context.PA_COD_EMPRESA);
            }
        }

        public override string Columna
        {
            get
            {
                return "Código de empresa";
            }
        }

        public override bool Requirement
        {
            get
            {
                var empresas = (List<EmpresaModel>)inputObject;
                int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                EmpresaModel result = empresas.FirstOrDefault(c => c.CE_COD_EMPRESA.Trim() == Context.PA_COD_EMPRESA.Trim() && c.CE_ESTATUS== activo.ToString());
                if (result == null)
                    Debug.Print("Vacio");
                Debug.Print(Context.PA_COD_EMPRESA);
                return result != null ? true : false;
            }
        }
    }
}
