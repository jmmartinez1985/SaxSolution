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
using System.Transactions;
using EntityFramework.Utilities;


namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class Supervisor : RepositoryBase<SAX_SUPERVISOR>, ISupervisor
    {
        public Supervisor()
            : this(new SaxRepositoryContext())
        {
        }
        public Supervisor(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_SUPERVISOR, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<SAX_SUPERVISOR, bool>> SearchFilters(SAX_SUPERVISOR obj)
        {
            return x => x.SV_ID_SUPERVISOR == obj.SV_ID_SUPERVISOR;
        }
        public SAX_SUPERVISOR InsertSupervisor(SAX_SUPERVISOR supervisor)
        {
            DBModelEntities db = new DBModelEntities();
            var validaDuplicidad = from v in db.SAX_SUPERVISOR
                                   where v.CE_ID_EMPRESA == supervisor.CE_ID_EMPRESA
                                   && v.SV_ID_AREA == supervisor.SV_ID_AREA
                                   select v;
            if (validaDuplicidad.Count() <= 0)
            {
                var objSupervisor = new Supervisor();
                var objSupervisorTemp = new SupervisorTemp();
                var paramInserted = new SAX_SUPERVISOR();

                using (var trx = new TransactionScope())
                {
                    supervisor.SV_ESTATUS = 0;
                    paramInserted = objSupervisor.Insert(supervisor, true);
                    var paramTemp = MappingTemp(paramInserted);
                    paramTemp.SV_ESTATUS = 2;
                    paramTemp = objSupervisorTemp.Insert(paramTemp, true);

                    trx.Complete();
                }
                return paramInserted;
            }
            else
            {
                var paramInserted = new SAX_SUPERVISOR();
                paramInserted = null;
                return paramInserted;
            }
        }
        private SAX_SUPERVISOR_TEMP MappingTemp(SAX_SUPERVISOR param)
        {
            var temp = new SAX_SUPERVISOR_TEMP();

            temp.SV_ID_SUPERVISOR = param.SV_ID_SUPERVISOR;
            temp.SV_ID_AREA = param.SV_ID_AREA;
            temp.CE_ID_EMPRESA = param.CE_ID_EMPRESA;
            temp.SV_COD_SUPERVISOR = param.SV_COD_SUPERVISOR;
            temp.SV_LIMITE_MINIMO = param.SV_LIMITE_MINIMO;
            temp.SV_LIMITE_SUPERIOR = param.SV_LIMITE_SUPERIOR;
            //temp.SV_TIPO_ACCION = param.SV_TIPO_ACCION;
            //temp.SV_ESTATUS_ACCION = param.SV_ESTATUS_ACCION;
            temp.SV_ESTATUS = param.SV_ESTATUS;
            temp.SV_FECHA_CREACION = param.SV_FECHA_CREACION;
            temp.SV_USUARIO_CREACION = param.SV_USUARIO_CREACION;
            temp.SV_FECHA_MOD = param.SV_FECHA_MOD;
            temp.SV_USUARIO_MOD = param.SV_USUARIO_MOD;
            temp.SV_FECHA_APROBACION = param.SV_FECHA_APROBACION;
            temp.SV_USUARIO_APROBADOR = param.SV_USUARIO_APROBADOR;
            
            return temp;
        }

    }
}
