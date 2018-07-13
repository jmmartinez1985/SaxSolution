using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    public class FOValidations: ValidationBase<PartidasModel>
    {
        private IRegistroControlService registroService;
        public FOValidations(PartidasModel context, object objectData) : base(context, objectData)
        {
        }


        public override string Columna
        {
            get
            {
                return "Fecha carga";
            }
        }
        public override string Message
        {
            get
            {
                return string.Format("La fecha de carga no puede ser mayor a la fecha operativa del sistema");
            }
        }

        public override bool Requirement
        {
            get
            {
                var fechaCarga = Context.PA_FECHA_CARGA.Date;
                var fechaOperativa = (DateTime)inputObject;
                return (fechaCarga <= fechaOperativa);
            }
        }
    }
}
