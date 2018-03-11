using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Rules
{
    public interface IValidationList
    {
        bool IsValid { get; }
        void Validate();
        IEnumerable<string> Messages { get; }
    }
}
