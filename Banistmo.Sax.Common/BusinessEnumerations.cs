using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Common
{
    public class BusinessEnumerations
    {
        public enum Estatus
        {
            ACTIVO = 1,
            INACTIVO = 0,
            ELIMINADO = 2
        }

        public enum TipoOperacion
        {
            CARGA_INICIAL = 21,
            CAPTURA_MANUAL = 22,
            CARGA_MASIVA = 23,
            CONCILIACION = 24,
            ANULACION = 25
        }

        public enum EstatusCarga
        {
            CREADO = 31,
            POR_APROBAR = 32,
            APROBADO = 33,
            ANULADO = 34,
            RECHAZADO = 35,
            ERRADO = 36,
            CONCILIADO = 37,
            POR_CONCILIAR = 99,
        }

        public enum TipoConciliacion
        {
            SI = 51,
            NO = 52
        }

        public enum TipoReferencia
        {
            AUTOMATICO = 61,
            MANUAL = 62
        }

        public enum Frecuencia
        {
            LUNES_VIERNES = 71,
            MARTES_SABADO = 72,
            LUNES_SABADO = 73,
            LUNES_DOMINGO = 74,
            DOMINGO_VIERNES = 75,
            DIARIO = 76,
            SEMANAL = 77,
            BIMESTRAL = 78,
            TRIMESTRAL = 79,
            ANUAL = 710,
            LUNES = 711,
            MARTES = 712,
            MIERCOLES = 713,
            JUEVES = 714,
            VIERNES = 715,
            SABADO = 716,
            DOMINGO = 717
        }

        public enum TipoAccion
        {
            CREADO = 81,
            EDITADO = 82,
            ELIMINADO = 83,
            ACTIVO = 84,
            INACTIVO = 85
        }

        public enum EstatusAccion
        {
            POR_APROBAR = 91,
            APROBADOR = 92,
            RECHAZADO = 93,
        }

        public enum Naturaleza
        {
            CREDITO = 94,
            DEBITO = 95
        }
    }
}
