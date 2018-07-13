using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
   public class IMPOValidations : ValidationBase<PartidasModel>
    {
        //Validacion para le importe, el importe no puede tener mas de 15 enteros y dos decimales.
        private IRegistroControlService registroService;
        private string mensaje;
        public IMPOValidations(PartidasModel context, object objectData) : base(context, objectData)
        {
        }


        public override string Columna
        {
            get
            {
                return "Importe";
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
                var importe = Context.PA_IMPORTE;
                if (importe != 0)
                {
                    decimal importeABS = Math.Abs(importe);
                    string importeString = importeABS.ToString();
                    if (!string.IsNullOrEmpty(importeString))
                    {
                        if (importeString.IndexOf(".") != -1)
                        {
                            string enteros = importeString.Substring(0, importeString.IndexOf("."));
                            string decimales = importeString.Substring(importeString.IndexOf(".") + 1, (importeString.Length - importeString.IndexOf(".") - 1));
                            if (enteros.Length > 15)
                            {
                                mensaje = "El importe indicado excede el formato permitido  (15 enteros y 2 decimales)";
                                return false;
                            }
                           
                            if (decimales.Length > 2)
                            {
                                mensaje = "El importe indicado excede el formato permitido  (15 enteros y 2 decimales)";
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            mensaje = "El importe indicado excede el formato permitido  (15 enteros y 2 decimales)";
                            return !(importeString.Length > 15);
                        }
                    }
                    else {
                        mensaje = "Debe colocar un valor en el importe";
                        return false;
                    }
                }
                else {
                    mensaje = "El importe no puede ser cero.";
                    return false;
                }
            }
        }
    }
}
