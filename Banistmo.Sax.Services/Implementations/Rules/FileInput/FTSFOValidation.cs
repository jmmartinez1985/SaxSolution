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
        private string mensaje;
        private string columna;
        private DateTime fechaOperativa;
        public FTSFOValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
            fechaOperativa = (DateTime)objectData;
            mensaje = "La fecha de transacción es mayor a la fecha de carga.";
            columna = "Fecha de transacción";
        }

        public override string Columna
        {
            get
            {
                return columna;
            }
        }
        public override string Message
        {
            get
            {
                return this.mensaje;
            }
        }

        public override bool Requirement
        {
            get
            {
                if (Context.PA_FECHA_TRX == default(DateTime) || Context.PA_FECHA_CARGA == default(DateTime))
                {
                    return true;
                }

                if (Context.PA_FECHA_CARGA.Date != fechaOperativa.Date) {
                    columna = "Fecha de carga";
                    mensaje = "La fecha de carga no es igual a la fecha de operación del sistema.";
                    return false;
                }

                return Context.PA_FECHA_TRX <= Context.PA_FECHA_CARGA;
            }
        }
    }
}
