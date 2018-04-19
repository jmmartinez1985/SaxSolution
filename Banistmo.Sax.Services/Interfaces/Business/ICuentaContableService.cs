using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;


namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface ICuentaContableService : IService<CuentaContableModel, SAX_CUENTA_CONTABLE, ICuentaContable>
    {
        List<CuentaContableModel> ConsultaCuentaCr();

        List<CuentaContableModel> ConsultaCuentaDb();
    }
}
