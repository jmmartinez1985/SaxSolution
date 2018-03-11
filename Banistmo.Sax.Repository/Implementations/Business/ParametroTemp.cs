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
    public class ParametroTemp : RepositoryBase<SAX_PARAMETRO_TEMP>, IParametroTemp
    {
        public ParametroTemp()
            : this(new SaxRepositoryContext())
        {
        }
        public ParametroTemp(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_PARAMETRO_TEMP, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<SAX_PARAMETRO_TEMP, bool>> SearchFilters(SAX_PARAMETRO_TEMP obj)
        {
            return x => x.PA_ID_PARAMETRO_TEMP == obj.PA_ID_PARAMETRO_TEMP;
        }

    }
}
