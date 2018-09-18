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
        bool AnularComprobante(int comprobante, List<string> empresas, string userName);

        void SolitarAnulacion(ComprobanteModel comprobante, string userName);

        void RechazarAnulacion(ComprobanteModel comprobante, string userName);

        bool ConciliacionManual(List<int> partidas, string userName);

        IQueryable<SAX_COMPROBANTE> ConsultaComprobanteConciliadaServ(DateTime? FechaCreacion,
                                                                        string empresaCod,
                                                                        int? comprobanteId,
                                                                        int? cuentaContableId,
                                                                        string cuentaContable,
                                                                        decimal? importe,
                                                                        string referencia,
                                                                        int? areaOpe,
                                                                        string lote,
                                                                        string capturador,
                                                                        int? statusCondi);

        IQueryable<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId);

        bool SolicitarAnulaciones(List<int> comprobantes, string userName);

        bool AprobarComprobante(int idComprobante, string userName);

        bool RechazarComprobante(int idComprobante, string userName);
    }
}
