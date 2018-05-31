using Banistmo.Sax.Services.Interfaces.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules
{
    public abstract class ValidationBase<T> : IValidation where T : class
    {
        protected T Context { get; private set; }
        protected List<T> ListRaw { get; private set; }

        protected ValidationBase(T context, object objectData)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            inputObject = objectData;
        }


        protected ValidationBase(T context, object objectData, List<T>  lista )
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            inputObject = objectData;
            ListRaw = lista;
        } 

        public void Validate()
        {
            if (!IsValid)
            {
                throw new ValidationException(Message);
            }
        }

        public virtual bool Condition
        {
            get
            {
                return true;
            }
        }

        public abstract bool Requirement { get; }

        public bool IsValid { get { return Implication(Condition, Requirement); } }

        public abstract string Message { get; }

        public object inputObject
        {
            get; set;
        }

        private static bool Implication(bool condition, bool requirement)
        {
            return !condition || requirement;
        }
    }
}
