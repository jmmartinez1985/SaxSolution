using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules
{
    public class ValidationException : Exception
    {
        public ValidationException(string message, params object[] args)
            : base(String.Format(message, args))
        {
        }
    }
}
