using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Results
{
    public class FileResult : IHttpActionResult
    {
        private readonly byte[] _filecontentType;
        private readonly string _contentType;

        public FileResult(byte[] fileContent, string contentType = null)
        {
            if (fileContent == null) throw new ArgumentNullException("filePath");
            _filecontentType = fileContent;
        }


        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            MemoryStream ms = new MemoryStream(_filecontentType);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ms)
            };
            var contentType = _contentType ?? MimeMapping.GetMimeMapping("xlsx");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return Task.FromResult(response);
        }
    }
}