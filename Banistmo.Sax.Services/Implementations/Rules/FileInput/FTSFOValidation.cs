using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de fecha de transaccion -> Fecha Transaccion Superior a Fecha de Operation
    /// </summary>
    public class FTSFOValidation : ValidationBase<PartidasModel>
    {
        public FTSFOValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }

        public override string Columna
        {
            get
            {
                return  "Fecha de transacción";
            }
        }
        public override string Message
        {
            get
            {
                return string.Format("La fecha de transacción es mayor a la fecha de carga.");
            }
        }

        public override bool Requirement
        {
            get
            {
                return Context.PA_FECHA_TRX <= Context.PA_FECHA_CARGA;
            }
        }
    }
}
