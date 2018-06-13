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
    public class PartidasAprobadasService : ServiceBase<PartidasAprobadasModel, vi_PartidasAprobadas, PartidasAprobadas>, IPartidasAprobadasService
    {
        private IPartidasAprobadas IParam;
        public PartidasAprobadasService()
            : this(new PartidasAprobadas())
        {

        }
        public PartidasAprobadasService(PartidasAprobadas dfs)
            : base(dfs)
        { }

        public PartidasAprobadasService(IPartidasAprobadas objIPar)
      : this(new PartidasAprobadas())
        {
            IParam = objIPar;
        }


        public List<PartidasAprobadasModel> ConsultaPartidaPorAprobar(string codEnterprise,
                                                                        string reference,
                                                                        decimal? importe,
                                                                        DateTime? trxDateIni,
                                                                        DateTime? trxDateFin,
                                                                        string ctaAccount,
                                                                        int? userArea)
        {
            //var model = Mapper.Map<PartidasAprobadasModel, vi_PartidasAprobadas>(param);
            //IParam = IParam ?? new Parametro();
            //var modelresult = IParam.InsertParametro(model);

            IParam = IParam ?? new PartidasAprobadas();
            var modelPar = IParam.ConsultaPartidaPorAprobar(codEnterprise,
                                                    reference,
                                                    importe,
                                                    trxDateIni,
                                                    trxDateFin,
                                                    ctaAccount,
                                                    userArea);
            return Mapper.Map<List<vi_PartidasAprobadas>, List<PartidasAprobadasModel>>(modelPar);
        }
    }
}
