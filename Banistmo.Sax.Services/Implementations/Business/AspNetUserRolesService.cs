using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using AutoMapper;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class AspNetUserRolesService : ServiceBase<AspNetUserRolesModel, AspNetUserRoles, AspNetUserRolesClass>, IAspNetUserRolesService
    {
        private IAspNetUserRoles areService;
        public AspNetUserRolesService()
            : this(new AspNetUserRolesClass())
        {

        }
        public AspNetUserRolesService(AspNetUserRolesClass ue)
            : base(ue)
        { }

        public AspNetUserRolesService(IAspNetUserRoles service)
        : this(new AspNetUserRolesClass())
        {
            areService = service;
        }

        public void CreateAndRemove(List<AspNetUserRolesModel> create, List<int> remove)
        {
            areService = areService != null ? areService : new AspNetUserRolesClass();
            List<AspNetUserRoles> modelA = Mapper.Map<List<AspNetUserRolesModel>, List<AspNetUserRoles>>(create);
            areService.CreateAndRemove(modelA, remove);
        }
    }
}
