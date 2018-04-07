﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IAspNetUserRolesService : IService<AspNetUserRolesModel, AspNetUserRoles, IAspNetUserRoles>
    {
        void CreateAndRemove(List<AspNetUserRolesModel> create, List<int> remove);
    }
}