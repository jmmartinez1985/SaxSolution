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
    public class EmpresaCentro : RepositoryBase<SAX_EMPRESA_CENTRO>, IEmpresaCentro
    {
        public EmpresaCentro()
            : this(new SaxRepositoryContext())
        {
        }
        public EmpresaCentro(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_EMPRESA_CENTRO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_EMPRESA_CENTRO, bool>> SearchFilters(SAX_EMPRESA_CENTRO obj)
        {
            return x => x.EC_ID_REGISTRO == obj.EC_ID_REGISTRO;
        }
    }
}
