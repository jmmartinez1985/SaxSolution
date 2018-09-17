using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces;
using System.Linq.Expressions;



namespace Banistmo.Sax.Repository.Implementations.Business
{
    public class ReportePartidasApr : RepositoryBase<vi_PartidasApr>, IReportePartidasApr
    {
        public override Expression<Func<vi_PartidasApr, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<vi_PartidasApr, bool>> SearchFilters(vi_PartidasApr obj)
        {
            throw new NotImplementedException();
        }
    }
}
