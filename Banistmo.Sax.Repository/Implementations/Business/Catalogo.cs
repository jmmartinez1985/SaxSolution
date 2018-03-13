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
    public class Catalogo : RepositoryBase<SAX_CATALOGO>, ICatalogo
    {
        public Catalogo()
            : this(new SaxRepositoryContext())
        {
        }
        public Catalogo(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_CATALOGO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_CATALOGO, bool>> SearchFilters(SAX_CATALOGO obj)
        {
            return x => x.CA_ID_CATALOGO == obj.CA_ID_CATALOGO;
        }
    }
}
