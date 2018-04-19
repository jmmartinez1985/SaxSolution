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
    public class CuentaContable : RepositoryBase<SAX_CUENTA_CONTABLE>, ICuentaContable
    {
        public CuentaContable()
            : this(new SaxRepositoryContext())
        {
        }
        public CuentaContable(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_CUENTA_CONTABLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_CUENTA_CONTABLE, bool>> SearchFilters(SAX_CUENTA_CONTABLE obj)
        {
            return x => x.CO_ID_CUENTA_CONTABLE == obj.CO_ID_CUENTA_CONTABLE;
        }

        public List<SAX_CUENTA_CONTABLE> ConsultaCuentaDb()
        {
            try
            {
                using (var db = new DBModelEntities())
                {
                    var ctaCont = (from a in db.SAX_CUENTA_CONTABLE
                                  where a.CO_COD_NATURALEZA == "D"
                                  select a).ToList();
                    return ctaCont;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede obtener las cuentas débitos." + ex.Message);
            }            
        }

        public List<SAX_CUENTA_CONTABLE> ConsultaCuentaCr()
        {
            try
            {
                using (var db = new DBModelEntities())
                {
                    var ctaCont = (from a in db.SAX_CUENTA_CONTABLE
                                   where a.CO_COD_NATURALEZA == "C"
                                   select a).ToList();
                    return ctaCont;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede obtener las cuentas débitos." + ex.Message);
            }
        }

        
    }
}
