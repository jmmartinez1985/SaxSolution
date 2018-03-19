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
    public class SupervisorTemp : RepositoryBase<SAX_SUPERVISOR_TEMP >, ISupervisorTemp
    {
        public SupervisorTemp()
            : this(new SaxRepositoryContext())
        {
        }
        public SupervisorTemp(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_SUPERVISOR_TEMP, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<SAX_SUPERVISOR_TEMP, bool>> SearchFilters(SAX_SUPERVISOR_TEMP obj)
        {
            return x => x.SV_ID_SUPERVISOR == obj.SV_ID_SUPERVISOR;
        }
    }
}
