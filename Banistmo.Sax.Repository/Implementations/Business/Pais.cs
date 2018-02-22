using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    public class Pais : RepositoryBase<PA_PAISES>, IPais
    {
        public Pais()
            : this(new SaxRepositoryContext())
        {
        }
        public Pais(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<PA_PAISES, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<PA_PAISES, bool>> SearchFilters(PA_PAISES obj)
        {
            throw new NotImplementedException();
        }
    }
}
