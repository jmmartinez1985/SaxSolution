using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ResponseResult
    {
        public List<string> Errors { get; set; }

        public object Result { get; set; }

        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public ResponseResult(object result, List<string> errors)
        {
            Errors = errors;
            Result = result;
        }

        public ResponseResult(List<string> errors, object result, int statusCode, string statusDescription)
        {
            Errors = errors;
            Result = result;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
    }
}
