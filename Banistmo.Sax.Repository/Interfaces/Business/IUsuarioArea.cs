using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IUsuarioArea : IRepository<SAX_USUARIO_AREA>
    {
        void CreateAndRemove(List<SAX_USUARIO_AREA> create, List<int> remove);
        
    }
}
