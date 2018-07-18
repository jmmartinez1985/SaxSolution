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

        string IsValidReferencia(string referencia, string empresa, string moneda, string cuenta_contable,string centro_costo, decimal monto_saldo, ref decimal monto, ref int tipo_error);

        bool removeRegistro (int registro);

        bool AprobarRegistro(int registro, string userName);

        bool RechazarRegistro(int registro, string userName);


    }
}
