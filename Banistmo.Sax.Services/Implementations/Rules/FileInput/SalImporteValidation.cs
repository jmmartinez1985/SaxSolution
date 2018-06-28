using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de saldo importe no puede ser 0 o en blanco
    /// </summary>
    public class SalImporteValidation : ValidationBase<PartidasModel>
    {
       
        public SalImporteValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
           
        }
        public override string Message
        {
            get
            {
                return "El importe no puede ser cero.";
            }
        }

        public override string Columna
        {
            get
            {
                return "Importe ";
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
