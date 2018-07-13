using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Helpers;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
 
    public interface IFilesProvider
    {
        PartidasContent cargaInicial<T>(T input, string userId, int areaId = 999);

        PartidasContent cargaMasiva<T>(T input, string userId, int areaId= 999 );

        void ValidaReglasCarga(int counter, ref List<PartidasModel> list, ref List<MessageErrorPartida> listError, PartidasModel partidaModel,
            int carga, List<CentroCostoModel> centroCostos, List<ConceptoCostoModel> conCostos,
            List<CuentaContableModel> ctaContables,
            List<EmpresaModel> empresa,
            List<PartidasModel> partidas,
            List<MonedaModel> monedas,
            DateTime fechaOperativa);
    }
}
