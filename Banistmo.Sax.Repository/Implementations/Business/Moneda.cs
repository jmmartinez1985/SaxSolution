using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class Moneda : RepositoryBase<SAX_MONEDA>, IMoneda
    {
        public Moneda()
            : this(new SaxRepositoryContext())
        {
        }
        public Moneda(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_MONEDA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_MONEDA, bool>> SearchFilters(SAX_MONEDA obj)
        {
            throw new NotImplementedException();
        }
    }
}
