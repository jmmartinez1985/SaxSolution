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
    public class Reporte : RepositoryBase<SAX_REPORTE_USUARIO_Result>, IReporte
    {
        public Reporte()
            : this(new SaxRepositoryContext()) { }
        public Reporte(IRepositoryContext repositoryContext)
            : base(repositoryContext) { }


        public override Expression<Func<SAX_REPORTE_USUARIO_Result, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REPORTE_USUARIO_Result, bool>> SearchFilters(SAX_REPORTE_USUARIO_Result obj)
        {
            throw new NotImplementedException();
        }
    }
}
