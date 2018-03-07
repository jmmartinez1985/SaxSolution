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
    public class Eventos : RepositoryBase<SAX_EVENTO>, IEventos
    {
        public Eventos()
            : this(new SaxRepositoryContext())
        {
        }
        public Eventos(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_EVENTO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_EVENTO, bool>> SearchFilters(SAX_EVENTO obj)
        {
            return x => x.EV_COD_EVENTO == obj.EV_COD_EVENTO;
        }
    }
}
