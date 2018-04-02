using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Services.Interfaces;
using System.Linq.Expressions;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class UsuariosPorRolService : ServiceBase<UsuariosPorRolModel, SAX_USUARIOS_POR_ROL_Result, UsuariosPorRol>, IUsuariosPorRoleService
    {

        public UsuariosPorRolService()
            : this(new UsuariosPorRol())
        {

        }
        public UsuariosPorRolService(UsuariosPorRol usrrol)
            : base(usrrol)
        { }

        public void Add(UsuarioRolModel entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(UsuarioRolModel entity)
        {
            throw new NotImplementedException();
        }

        public List<UsuariosPorRolModel> GetReporte()
        {
            return this.ExecuteProcedure("SAX_USUARIOS_POR_ROL", new object[0]);
        }

        public void Insert(UsuarioRolModel entity)
        {
            throw new NotImplementedException();
        }

        public UsuarioRolModel Insert(UsuarioRolModel entity, bool status)
        {
            throw new NotImplementedException();
        }

        public void Update(UsuarioRolModel entity)
        {
            throw new NotImplementedException();
        }

        List<UsuarioRolModel> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.ExecuteProcedure(string spName, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        List<UsuarioRolModel> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAll()
        {
            throw new NotImplementedException();
        }

        List<UsuarioRolModel> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAll(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> whereCondition)
        {
            throw new NotImplementedException();
        }

        IList<UsuarioRolModel> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAll(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> filter, Func<IQueryable<SAX_USUARIOS_POR_ROL_Result>, IOrderedQueryable<SAX_USUARIOS_POR_ROL_Result>> orderBy, params Expression<Func<SAX_USUARIOS_POR_ROL_Result, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<UsuarioRolModel>> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<ICollection<UsuarioRolModel>> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAllAsync(params Expression<Func<SAX_USUARIOS_POR_ROL_Result, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<UsuarioRolModel>> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAllAsync(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> whereCondition)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<UsuarioRolModel>> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetAllAsync(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> whereCondition, params Expression<Func<SAX_USUARIOS_POR_ROL_Result, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        UsuarioRolModel IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetSingle(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> whereCondition)
        {
            throw new NotImplementedException();
        }

        UsuarioRolModel IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetSingle(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> filter, params Expression<Func<SAX_USUARIOS_POR_ROL_Result, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        Task<UsuarioRolModel> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetSingleAsync(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> whereCondition)
        {
            throw new NotImplementedException();
        }

        Task<UsuarioRolModel> IService<UsuarioRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>.GetSingleAsync(Expression<Func<SAX_USUARIOS_POR_ROL_Result, bool>> whereCondition, params Expression<Func<SAX_USUARIOS_POR_ROL_Result, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
