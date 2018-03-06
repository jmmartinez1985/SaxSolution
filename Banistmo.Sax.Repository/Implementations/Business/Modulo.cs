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
    public class Modulo : RepositoryBase<SAX_MODULO>, IModulo
    {
        public Modulo()
            : this(new SaxRepositoryContext())
        {
        }
        public Modulo(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_MODULO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_MODULO, bool>> SearchFilters(SAX_MODULO obj)
        {
            return x => x.MO_ID_MODULO == obj.MO_ID_MODULO;
        }
    }
}
