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
    public class UsuarioRol : RepositoryBase<SAX_USUARIO_ROL>, IUsuarioRol
    {
        public UsuarioRol()
            : this(new SaxRepositoryContext())
        {
        }
        public UsuarioRol(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_USUARIO_ROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_USUARIO_ROL, bool>> SearchFilters(SAX_USUARIO_ROL obj)
        {
            return x => x.UR_ID_USUARIO_ROL == obj.UR_ID_USUARIO_ROL;
        }

    }

}
