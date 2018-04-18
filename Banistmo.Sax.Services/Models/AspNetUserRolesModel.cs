using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class AspNetUserRolesModel
    {
        public int IDAspNetUserRol { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public AspNetRoles AspNetRoles { get; set; }
        public AspNetUsers AspNetUsers { get; set; }
    }
}
