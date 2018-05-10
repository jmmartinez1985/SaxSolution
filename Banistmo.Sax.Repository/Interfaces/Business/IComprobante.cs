using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IComprobante : IRepository<SAX_COMPROBANTE>
    {
        bool AnularComprobante(int comprobante, string userName);

    }
}
