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
    public class User : RepositoryBase<SAX_USUARIO>, IUser
    {
        public User()
            : this(new SaxRepositoryContext())
        {
        }
        public User(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_USUARIO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_USUARIO, bool>> SearchFilters(SAX_USUARIO obj)
        {
            return x => x.US_COD_USUARIO == obj.US_COD_USUARIO;
        }
    }
}
