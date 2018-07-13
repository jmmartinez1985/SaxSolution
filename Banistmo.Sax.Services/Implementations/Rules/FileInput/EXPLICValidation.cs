using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    public class EXPLICValidation :ValidationBase<PartidasModel>
    {

        public EXPLICValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }


        public override string Columna
        {
            get
            {
                return "Explicación de la transacción ";
            }
        }
        public override string Message
        {
            get
            {
                if(string.IsNullOrEmpty(Context.PA_EXPLICACION))
                    return string.Format("La explicación de la transacción no puede estar vacía.");
                else
                    return string.Format("La explicación de la transacción no es valida");
            }
        }

        public override bool Requirement
        {
            get
            {
                if (String.IsNullOrEmpty(Context.PA_EXPLICACION))
                {
                    return false;
                }
                else {
                    string cadena= Regex.Replace(Context.PA_EXPLICACION, "[^0-9a-zA-Z]+", "");
                    if (string.IsNullOrEmpty(cadena))
                    {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
        }
    }
}
