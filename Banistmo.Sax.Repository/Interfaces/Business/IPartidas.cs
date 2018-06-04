using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
   public  interface IPartidas : IRepository<SAX_PARTIDAS>
    {
        //List<Model.vi_PartidasAprobadas> ConsultaConciliacioneManualPorAprobar(DateTime? Fechatrx,
        //                                                                string empresaCod,
        //                                                                int? areaPartida);
    }
}
