using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IRegistroControl : IRepository<SAX_REGISTRO_CONTROL>
    {
        SAX_REGISTRO_CONTROL LoadFileData(SAX_REGISTRO_CONTROL control);

        bool IsValidLoad(DateTime fecha);

        string IsValidReferencia(string referencia, ref decimal monto);

        bool removeRegistro (int registro);

        bool AprobarRegistro(int registro, string userName);

        bool RechazarRegistro(int registro, string userName);


    }
}
