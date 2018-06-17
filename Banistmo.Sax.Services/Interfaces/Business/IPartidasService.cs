using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IPartidasService : IService<PartidasModel, SAX_PARTIDAS, IPartidas>
    {

        bool isSaldoValidoMoneda(List<PartidasModel> partidas, ref List<MonedaValidationModel> monedasValid);

        bool isSaldoValidoEmpresa(List<PartidasModel> partidas, ref List<EmpresaValidationModel> monedasValid);

        bool isSaldoValidoMonedaEmpresa(List<PartidasModel> partidas, ref List<EmpresaMonedaValidationModel> monedasValid);

        List<ReferenceGroupingModel> getConsolidaReferencias(List<PartidasModel> partidas);

    }
}
