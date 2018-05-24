using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de saldo de cuenta 61 y 64
    /// </summary>
    public class SALCTAValidation : ValidationBase<PartidasModel>
    {

        public SALCTAValidation(PartidasModel context, object objectData) : base(context, objectData)
        {

        }

        public override string Message
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool Requirement
        {
            get
            {
                var partidas = (List<PartidasModel>)inputObject;

                if (Context.PA_CTA_CONTABLE.StartsWith("61"))
                {
                    var contains = Context.PA_CTA_CONTABLE.Remove(0, 1);
                    var validator = "64" + contains;
                    var first = partidas.FirstOrDefault(c => c.PA_CTA_CONTABLE == validator);
                    if (first != null)
                    {
                        if (Context.PA_IMPORTE + first.PA_IMPORTE == 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else if (Context.PA_CTA_CONTABLE.StartsWith("62"))
                {
                    var contains = Context.PA_CTA_CONTABLE.Remove(0, 1);
                    var validator = "65" + contains;
                    var first = partidas.FirstOrDefault(c => c.PA_CTA_CONTABLE == validator);
                    if (first != null)
                    {
                        if (Context.PA_IMPORTE + first.PA_IMPORTE == 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else if (Context.PA_CTA_CONTABLE.StartsWith("63"))
                {
                    var contains = Context.PA_CTA_CONTABLE.Remove(0, 1);
                    var validator = "66" + contains;
                    var first = partidas.FirstOrDefault(c => c.PA_CTA_CONTABLE == validator);
                    if (first != null)
                    {
                        if (Context.PA_IMPORTE + first.PA_IMPORTE == 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
        }
    }
}
