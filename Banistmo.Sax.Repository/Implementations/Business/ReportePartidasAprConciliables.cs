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
    [Injectable]
  public class ReportePartidasAprConciliables : RepositoryBase<vi_PartidasApr_Conciliadas >, IReportePartidasAprConciliables
    {
        public ReportePartidasAprConciliables()
            :this(new SaxRepositoryContext())
        { }

        public ReportePartidasAprConciliables(IRepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public override Expression<Func<vi_PartidasApr_Conciliadas, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<vi_PartidasApr_Conciliadas, bool>> SearchFilters(vi_PartidasApr_Conciliadas obj)
        {
            throw new NotImplementedException();
        }
    }
}
