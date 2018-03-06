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
    public class Empresa : RepositoryBase<SAX_EMPRESA>, IEmpresa
    {
        public Empresa()
            : this(new SaxRepositoryContext())
        {
        }
        public Empresa(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_EMPRESA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_EMPRESA, bool>> SearchFilters(SAX_EMPRESA obj)
        {
            return x => x.CE_ID_EMPRESA == obj.CE_ID_EMPRESA;
        }
    }
}
