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
    public class Partidas : RepositoryBase<SAX_PARTIDAS>, IPartidas
    {
        public Partidas()
            : this(new SaxRepositoryContext())
        {
        }
        public Partidas(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_PARTIDAS, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_PARTIDAS, bool>> SearchFilters(SAX_PARTIDAS obj)
        {
            return x => x.PA_REGISTRO == obj.PA_REGISTRO;
        }
    }
}
