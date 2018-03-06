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

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class RolesService : ServiceBase<RolesModel, AspNetRoles, Roles>, IRolesService
    {
        public RolesService()
            : this(new Roles())
        {

        }
        public RolesService(Roles rl)
            : base(rl)
        { }
    }
}
