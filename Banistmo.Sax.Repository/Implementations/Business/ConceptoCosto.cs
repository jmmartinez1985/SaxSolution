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
    public class ConceptoCosto : RepositoryBase<SAX_CONCEPTO_COSTO>, IConceptoCosto
    {
        public ConceptoCosto()
            : this(new SaxRepositoryContext())
        {
        }
        public ConceptoCosto(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_CONCEPTO_COSTO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_CONCEPTO_COSTO, bool>> SearchFilters(SAX_CONCEPTO_COSTO obj)
        {
            return x => x.CC_ID_CONCEPTO_COSTO == obj.CC_ID_CONCEPTO_COSTO;
        }
    }
}
