using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    public class EXPLICValidation :ValidationBase<PartidasModel>
    {

        public EXPLICValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format("La explicación de la transacción no puede estar vacía.");
            }
        }

        public override bool Requirement
        {
            get
            {
                return !String.IsNullOrEmpty(Context.PA_EXPLICACION);
            }
        }
    }
}
