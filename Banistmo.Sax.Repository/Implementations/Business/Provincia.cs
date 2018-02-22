using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    public class Provincia : RepositoryBase<PR_PROVINCIA>, IProvincia
    {
        public Provincia(): this(new SaxRepositoryContext())
        {
        }
        public Provincia(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<PR_PROVINCIA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<PR_PROVINCIA, bool>> SearchFilters(PR_PROVINCIA obj)
        {
            throw new NotImplementedException();
        }
    }
}
