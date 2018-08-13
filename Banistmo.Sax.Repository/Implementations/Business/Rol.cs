using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class Rol: RepositoryBase<AspNetRoles>, IRol
    {

        public Rol()
            : this(new SaxRepositoryContext())
        {
        }
        public Rol(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<AspNetRoles, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<AspNetRoles, bool>> SearchFilters(AspNetRoles obj)
        {
            throw new NotImplementedException();
        }
    }
}
