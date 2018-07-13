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
        private IRegistroControlService registroService;
        public FTFCIFOValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }


        public override string Columna
        {
            get
            {
                return "Fecha de transaccion";
            }
        }
        public override string Message
        {
            get
            {
                return string.Format("La fecha de transacción  o la fecha de carga  no son iguales a la fecha de operación {2}.");
            }
        }

        public override bool Requirement
        {
            get
            {
                registroService = new RegistroControlService();
                return ((registroService.IsValidLoad (Context.PA_FECHA_TRX.Date)) && (registroService.IsValidLoad( Context.PA_FECHA_CARGA.Date )));
            }
        }
    }
}
