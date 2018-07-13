﻿using Banistmo.Sax.Services.Models;
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
                return "Explicación de la transacción";
            }
        }
        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(Context.PA_EXPLICACION))
                    return string.Format("La explicación de la transacción no puede estar vacía.");
                else if (Context.PA_EXPLICACION.Length < 3) {
                    return string.Format("La explicación colocada tiene que tener más de 3 caracteres.");
                }
                else
                    return string.Format("La explicación de la transacción no es válida.");
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
                    string cadena = Context.PA_EXPLICACION.Trim();
                    cadena= Regex.Replace(Context.PA_EXPLICACION, "[^0-9a-zA-Z]+", "");
                    if (string.IsNullOrEmpty(cadena))
                    {
                        return false;
                    }
                    else if (cadena.Length < 3){
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
