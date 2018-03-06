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
    public class UsuarioEmpresa : RepositoryBase<SAX_USUARIO_EMPRESA>, IUsuarioEmpresa
    {
        public UsuarioEmpresa()
            : this(new SaxRepositoryContext())
        {
        }
        public UsuarioEmpresa(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_USUARIO_EMPRESA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_USUARIO_EMPRESA, bool>> SearchFilters(SAX_USUARIO_EMPRESA obj)
        {
            return x => x.UE_ID_USUARIO_EMPRESA == obj.UE_ID_USUARIO_EMPRESA;
        }

    }
}
