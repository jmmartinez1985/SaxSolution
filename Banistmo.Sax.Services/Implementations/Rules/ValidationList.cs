using Banistmo.Sax.Services.Interfaces.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules
{
    public class ValidationList : List<IValidation>, IValidationList
    {
        public bool IsValid
        {
            get
            {
                return this.All(v => v.IsValid);
            }
        }

        public void Validate()
        {
            foreach (var validation in this)
            {
                validation.Validate();
            }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                return this.Where(v => !v.IsValid).Select(v => v.Message);
            }
        }
    }
}
