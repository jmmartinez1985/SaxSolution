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
    public class EmpresaAreasCentroCosto : RepositoryBase<vi_EmpresaAreasCentroCosto>, IEmpresaAreasCentroCosto
    {
        public EmpresaAreasCentroCosto()
            : this(new SaxRepositoryContext())
        {
        }
        public EmpresaAreasCentroCosto(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<vi_EmpresaAreasCentroCosto, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<vi_EmpresaAreasCentroCosto, bool>> SearchFilters(vi_EmpresaAreasCentroCosto obj)
        {
            return x => x.AreaDesc == obj.AreaDesc;
        }
    }
}
