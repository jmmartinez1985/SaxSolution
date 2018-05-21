using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class PartidasAprobadas : RepositoryBase<vi_PartidasAprobadas>, IPartidasAprobadas
    {
        public PartidasAprobadas()
            : this(new SaxRepositoryContext())
        {
        }
        public PartidasAprobadas(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<vi_PartidasAprobadas, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<vi_PartidasAprobadas, bool>> SearchFilters(vi_PartidasAprobadas obj)
        {
            return x => x.AREAOPERATIVADESC == obj.AREAOPERATIVADESC;
        }

    }
}
