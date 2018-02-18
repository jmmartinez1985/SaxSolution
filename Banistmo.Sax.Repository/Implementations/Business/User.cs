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
//SA: JMMB
namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class User : RepositoryBase<USR_Users>, IUser
    {
        public User()
            : this(new SaxRepositoryContext())
        {
        }
        public User(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<USR_Users, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<USR_Users, bool>> SearchFilters(USR_Users obj)
        {
            return x => x.USR_Id == obj.USR_Id;
        }
    }
}
