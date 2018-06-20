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

        bool ConciliacionManual(List<int> partidas, string userName);

        IQueryable<SAX_COMPROBANTE> ConsultaComprobanteConciliada(DateTime? FechaCreacion,
                                                                        string empresaCod,
                                                                        int? comprobanteId,
                                                                        int? cuentaContableId,
                                                                        decimal? importe,
                                                                        string referencia,
                                                                        int? areaOpe);
        IQueryable<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId);

    }
}
