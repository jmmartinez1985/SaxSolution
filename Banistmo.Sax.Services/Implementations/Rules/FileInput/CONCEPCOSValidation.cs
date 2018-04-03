﻿using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    class CONCEPCOSValidation:ValidationBase<PartidasModel>
    {
        public CONCEPCOSValidation(PartidasModel context, object objectData) : base(context, objectData)
        {
        }
        public override string Message
        {
            get
            {
                return string.Format(@"El concepto de costo ""{0}"" no es válido.", Context.PA_CONCEPTO_COSTO);
            }
        }

        public override bool Requirement
        {
            get
            {
                var conceptos = (List<ConceptoCostoModel>)inputObject;
                ConceptoCostoModel result = conceptos.FirstOrDefault(c => c.CC_NUM_CONCEPTO == Context.PA_CONCEPTO_COSTO);
                return result != null ? true : false;
            }
        }
    }
}
