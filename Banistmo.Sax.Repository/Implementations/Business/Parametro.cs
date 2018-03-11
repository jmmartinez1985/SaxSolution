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
    public class Parametro : RepositoryBase<SAX_PARAMETRO>, IParametro
    {
        public Parametro()
            : this(new SaxRepositoryContext())
        {
        }
        public Parametro(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_PARAMETRO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<SAX_PARAMETRO, bool>> SearchFilters(SAX_PARAMETRO obj)
        {
            return x => x.PA_ID_PARAMETRO == obj.PA_ID_PARAMETRO;
        }

    }
}
