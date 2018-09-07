using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    class FTValidation:ValidationBase<PartidasModel>
    {
        private string columna;
        private string mensaje;
        private IRegistroControlService registroService;
        public FTValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }


        public override string Columna
        {
            get
            {
                return this.columna;
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
                if (Context.PA_FECHA_TRX == default(DateTime))
                {
                    return true;
                }
                var fechaOperativa = (DateTime)inputObject;
                if (Context.PA_FECHA_TRX.Date != fechaOperativa )
                {
                    columna = "Fecha de Transacción";
                    mensaje = "La fecha de transacción no es  igual a la fecha de operación.";
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
