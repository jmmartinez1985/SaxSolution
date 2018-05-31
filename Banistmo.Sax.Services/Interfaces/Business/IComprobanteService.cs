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
    public interface IComprobanteService : IService<ComprobanteModel, SAX_COMPROBANTE, IComprobante>
    {
        bool AnularComprobante(int comprobante, string userName);

        void SolitarAnulacion(ComprobanteModel comprobante, string userName);

        void RechazarAnulacion(ComprobanteModel comprobante, string userName);

        bool ConciliacionManual(List<int> partidas, string userName);

        List<ComprobanteModel> ConsultaComprobanteConciliadaServ(DateTime? FechaCreacion,
                                                                        int? empresaId,
                                                                        int? comprobanteId,
                                                                        int? cuentaContableId,
                                                                        double? importe,
                                                                        string referencia);

        List<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId);
    }
}
