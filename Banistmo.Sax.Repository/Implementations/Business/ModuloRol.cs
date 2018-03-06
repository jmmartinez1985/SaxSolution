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
    public class ModuloRol : RepositoryBase<SAX_MODULO_ROL>, IModuloRol
    {
        public ModuloRol()
            : this(new SaxRepositoryContext())
        {
        }
        public ModuloRol(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_MODULO_ROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_MODULO_ROL, bool>> SearchFilters(SAX_MODULO_ROL obj)
        {
            return x => x.MR_ID_MODULO_ROL == obj.MR_ID_MODULO_ROL;
        }
    }
}
