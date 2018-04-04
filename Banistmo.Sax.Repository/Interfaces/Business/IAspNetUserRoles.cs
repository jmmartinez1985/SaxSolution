using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IAspNetUserRoles : IRepository<AspNetUserRoles>
    {
        void CreateAndRemove(List<AspNetUserRoles> create, List<int> remove);
    }
}
