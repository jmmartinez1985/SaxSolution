using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IPartidasAprobadasService : IService<PartidasAprobadasModel, vi_PartidasAprobadas, IPartidasAprobadas>
    {
        IQueryable<vi_PartidasAprobadas> ConsultaPartidaPorAprobar(string codEnterprise,
                                                                        string reference,
                                                                        decimal? importe,
                                                                        DateTime? trxDateIni,
                                                                        DateTime? trxDateFin,
                                                                        string ctaAccount,
                                                                        int? userArea,
                                                                        decimal? importeDesde, 
                                                                        decimal? importeHasta);
    }
}
