using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Helpers;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IRegistroControlService : IService<RegistroControlModel, SAX_REGISTRO_CONTROL, IRegistroControl>
    {
        RegistroControlModel LoadFileData(RegistroControlModel control, List<PartidasModel> excelData, int tipoOperacion);

        RegistroControlContent CreateSinglePartidas(RegistroControlModel control, PartidaManualModel partida, int tipoOperacion);

        bool IsValidLoad(DateTime fecha);

        string IsValidReferencia(string referencia);

         string FileName { get; set; }
    }
}
