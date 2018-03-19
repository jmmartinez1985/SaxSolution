using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    class IValidation: ValidationBase<PartidasModel>
    {
       
        public IValidation(PartidasModel context) : base(context)
        {
           
        }
        public override string Message
        {
            get
            {
                return "El importe no puede ser cero.";
            }
        }

        public override bool Requirement
        {
            get
            {
               
                return Math.Abs(Context.PA_IMPORTE)>0;
            }
        }
    }
}
