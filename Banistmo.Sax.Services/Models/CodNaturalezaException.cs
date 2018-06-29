using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class CodNaturalezaException : Exception
    {
        public CodNaturalezaException() { }
        public CodNaturalezaException(string message):
            base(message)
        {

        }
    }

    public class CodConciliaException : Exception
    {
        public CodConciliaException() { }
        public CodConciliaException(string message):
            base(message)
        {

        }
    }

    public class CuentaContableException : Exception
    {
        public CuentaContableException() { }
        public CuentaContableException(string message) :
            base(message)
        {

        }
    }

    public class ReferenciaException : Exception
    {
        public ReferenciaException() { }
        public ReferenciaException(string message) :
            base(message)
        {

        }
    }

    public class ReferenciaInicialException : Exception
    {
        public ReferenciaInicialException() { }
        public ReferenciaInicialException(string message) :
            base(message)
        {

        }
    }

    public class CuentaContableAreaException : Exception
    {
        public CuentaContableAreaException() { }
        public CuentaContableAreaException(string message) :
            base(message)
        {

        }
    }

    public class EmpresaException : Exception
    {
        public EmpresaException() { }
        public EmpresaException(string message) :
            base(message)
        {

        }
    }
}
