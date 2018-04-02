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
    public class UsuariosPorRol : RepositoryBase<SAX_USUARIOS_POR_ROL_Result>, IUsuariosPorRol
    {
        public UsuariosPorRol()
            : this(new SaxRepositoryContext())
        { }

        public UsuariosPorRol(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> SearchFilters(SAX_USUARIOS_POR_ROL_Result obj)
        {
            throw new NotImplementedException();
        }
    }
}
