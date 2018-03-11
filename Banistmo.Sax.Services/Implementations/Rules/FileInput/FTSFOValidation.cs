using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    //Fecha Transaccion Superior a Fecha de Operation
    public class FTSFOValidation : ValidationBase<PartidasModel>
    {
        public FTSFOValidation(PartidasModel context) : base(context)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format("La fecha de transaccion {0} es mayor a la fecha de la operacion {1}.", Context.PA_FECHA_TRX.ToShortDateString(), Context.PA_FECHA_CARGA.ToShortDateString());
            }
        }

        public override bool Requirement
        {
            get
            {
                return Context.PA_FECHA_TRX < Context.PA_FECHA_CARGA;
            }
        }
    }
}
