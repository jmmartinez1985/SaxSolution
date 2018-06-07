﻿using System;
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
}
