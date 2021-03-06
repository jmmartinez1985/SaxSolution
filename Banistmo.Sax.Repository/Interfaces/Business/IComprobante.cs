﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IComprobante : IRepository<SAX_COMPROBANTE>
    {
        bool AnularComprobante(int comprobante, List<string> empresas, string userName);

        bool ConciliacionManual(List<int> partidas, string userName);

        IQueryable<SAX_COMPROBANTE> ConsultaComprobanteConciliada(DateTime? FechaCreacion,
                                                                        string empresaCod,
                                                                        int? comprobanteId,
                                                                        int? cuentaContableId,
                                                                        string cuentaContable,
                                                                        decimal? importe,
                                                                        string referencia,
                                                                        int? areaOpe,
                                                                        string lote,
                                                                        string capturador,
                                                                        int? statusCondi,
                                                                        string usuario);
        IQueryable<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId);

        bool SolicitarAnulaciones(List<int> comprobantes, string userName);

        bool AprobarComprobante(int idComprobante,List<string> empresas, string userName);

        bool RechazarComprobante(int idComprobante, string userName);

    }
}
