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
    public class Parametro : RepositoryBase<SAX_PARAMETRO>, IParametro
    {
        public Parametro()
            : this(new SaxRepositoryContext())
        {
        }
        public Parametro(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_PARAMETRO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<SAX_PARAMETRO, bool>> SearchFilters(SAX_PARAMETRO obj)
        {
            return x => x.PA_ID_PARAMETRO == obj.PA_ID_PARAMETRO;
        }
        public SAX_PARAMETRO InsertParametro(SAX_PARAMETRO param)
        {
            var objParam = new Parametro();
            var objParamTemp = new ParametroTemp();
            var paramInserted = new SAX_PARAMETRO();
           
            using (var trx = new TransactionScope())
            {
                param.PA_ESTATUS = 0;
                try
                { paramInserted = objParam.Insert(param, true);  }
                catch (Exception ex)
                {
                    var hi = ex.InnerException;
                }
                paramInserted = objParam.Insert(param, true);
                var paramTemp = MappingTemp(paramInserted);
                paramTemp.PA_ESTATUS = 2;
                paramTemp = objParamTemp.Insert(paramTemp, true);

                trx.Complete();
            }
            return paramInserted;
        }
        private SAX_PARAMETRO_TEMP MappingTemp(SAX_PARAMETRO param)
        {
            var temp = new SAX_PARAMETRO_TEMP();

            //temp.PA_COD_PARAMETRO = param.PA_COD_PARAMETRO;
            //temp.PA_DESCRIPCION = param.PA_DESCRIPCION;
            temp.PA_ESTATUS = param.PA_ESTATUS;
            //temp.PA_ESTATUS_ACCION = param.PA_ESTATUS_ACCION;
            temp.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            temp.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            temp.PA_FECHA_MOD = param.PA_FECHA_MOD;
            temp.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            //temp.PA_FILE_CONTABLE = param.PA_FILE_CONTABLE;
            temp.PA_FRECUENCIA = param.PA_FRECUENCIA;
            temp.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            temp.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            temp.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            temp.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            temp.PA_RUTA_TEMPORAL= param.PA_RUTA_TEMPORAL;
            //temp.PA_TIPO_ACCION = param.PA_TIPO_ACCION;
            temp.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            temp.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            temp.PA_USUARIO_MOD = param.PA_USUARIO_MOD;
            
            return temp;
        }

    }
}
