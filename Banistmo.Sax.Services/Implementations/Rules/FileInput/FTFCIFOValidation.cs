using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    //Fecha de transacción o fecha de carga igual a fecha operativa  
    class FTFCIFOValidation:ValidationBase<PartidasModel>
    {
        public FTFCIFOValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format("La fecha de transacción {0} o la fecha de carga {1} no son iguales a la fecha de operación {2}.", Context.PA_FECHA_TRX.ToShortDateString(), Context.PA_FECHA_CARGA.ToShortDateString(), DateTime.Now.Date.ToShortDateString());
            }
        }

        public override bool Requirement
        {
            get
            {
                return (Context.PA_FECHA_TRX == DateTime.Now.Date) && (Context.PA_FECHA_CARGA == DateTime.Now.Date);
            }
        }
    }
}
