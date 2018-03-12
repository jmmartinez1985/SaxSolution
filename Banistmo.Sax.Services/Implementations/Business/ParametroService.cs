using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations;
using Banistmo.Sax.Common;
using AutoMapper;
using Banistmo.Sax.Repository.Interfaces.Business;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ParametroService : ServiceBase<ParametroModel, SAX_PARAMETRO, Parametro>, IParametroService
    {

        private readonly IParametro IParam;

        public ParametroService()
            : this(new Parametro())
        {

        }

        public ParametroService(Parametro objPar)
            : base(objPar)
        { }

        public ParametroService(IParametro objIPar)
      : this(new Parametro())
        {
            IParam = objIPar;
        }

        public void InsertParametro(ParametroModel param)
        {
            var model = Mapper.Map<ParametroModel, SAX_PARAMETRO>(param);
            IParam.InsertParametro(model);

        }
    }
}
