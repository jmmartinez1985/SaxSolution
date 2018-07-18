using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de fecha de transaccion -> Fecha de transacción o fecha de carga igual a fecha operativa  
    /// </summary>
    public class FTFCIFOValidation:ValidationBase<PartidasModel>
    {
        private string columna;
        private string mensaje;
        private IRegistroControlService registroService;
        public FTFCIFOValidation(PartidasModel context, object objectData) : base(context, objectData)
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
                if (Context.PA_FECHA_TRX == default(DateTime) || Context.PA_FECHA_CARGA == default(DateTime)) {
                    return true;
                }
                var fechaOperativa = (DateTime)inputObject;
                if (Context.PA_FECHA_TRX.Date != fechaOperativa && Context.PA_FECHA_CARGA.Date != fechaOperativa)
                {
                    columna = "Fecha de carga / Fecha de Transacción";
                    mensaje = "La fecha de carga y la fecha de transacción no son iguales a la fecha de operacion.";
                    return false;
                }
                 else   if (Context.PA_FECHA_TRX.Date != fechaOperativa)
                {
                    columna = "Fecha de Transacción";
                    mensaje = "La fecha de transacción no es  igual a la fecha de operación.";
                    return false;
                }
                else if (Context.PA_FECHA_CARGA.Date != fechaOperativa)
                {
                    columna = "Fecha de carga";
                    mensaje = "La fecha de carga no es  igual a la fecha de operacin.";
                    return false;
                }
                else {
                    return true;
                }
            }
        }
    }
}
