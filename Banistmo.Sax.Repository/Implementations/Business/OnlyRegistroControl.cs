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
    public class OnlyRegistroControl: RepositoryBase<SAX_REGISTRO_CONTROL>,IOnlyRegistroControl
    {
        public OnlyRegistroControl()
            : this(new SaxRepositoryContext())
        {
        }
        public OnlyRegistroControl(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> SearchFilters(SAX_REGISTRO_CONTROL obj)
        {
            throw new NotImplementedException();
        }
    }
}
