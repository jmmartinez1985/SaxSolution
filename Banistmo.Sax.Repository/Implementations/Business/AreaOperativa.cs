using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
using System.Linq.Expressions;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class AreaOperativa : RepositoryBase<SAX_AREA_OPERATIVA>, IAreaOperativa
    {
        public AreaOperativa()
            : this(new SaxRepositoryContext())
        {
        }
        public AreaOperativa(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_AREA_OPERATIVA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_AREA_OPERATIVA, bool>> SearchFilters(SAX_AREA_OPERATIVA obj)
        {
            return x => x.CA_ID_AREA == obj.CA_ID_AREA;
        }
    }
   
}
