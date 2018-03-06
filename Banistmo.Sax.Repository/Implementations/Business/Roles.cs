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
    public class Roles : RepositoryBase<AspNetRoles>, IRoles
    {
        public Roles()
            : this(new SaxRepositoryContext())
        {
        }
        public Roles(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<AspNetRoles, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<AspNetRoles, bool>> SearchFilters(AspNetRoles obj)
        {
            return x => x.Id == obj.Id;
        }
    }
}
