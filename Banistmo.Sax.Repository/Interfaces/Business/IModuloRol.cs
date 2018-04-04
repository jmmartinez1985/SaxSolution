using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IModuloRol : IRepository<SAX_MODULO_ROL>
    {
        void CreateAndRemove(List<SAX_MODULO_ROL> create);
    }
}
