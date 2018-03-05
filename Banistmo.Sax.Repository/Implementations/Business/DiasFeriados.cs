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
    public class DiasFeriados : RepositoryBase<SAX_DIAS_FERIADOS>, IDiasFeriados
    {
        public DiasFeriados()
            : this(new SaxRepositoryContext())
        {
        }
        public DiasFeriados(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_DIAS_FERIADOS, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_DIAS_FERIADOS, bool>> SearchFilters(SAX_DIAS_FERIADOS obj)
        {
            return x => x.CD_ID_DIA_FERIADO == obj.CD_ID_DIA_FERIADO;
        }
    }
}
