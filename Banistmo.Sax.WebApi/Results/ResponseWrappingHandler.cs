using Banistmo.Sax.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Results
{
    public class ResponseWrappingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            return BuildApiResponse(request, response);
        }

        private HttpResponseMessage BuildApiResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            object content;
            if (response.Content !=null && response.Content.Headers.ContentType.MediaType == "application/octet-stream")
                return response;
            List<string> modelStateErrors = new List<string>();
            if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
            {
                HttpError error = content as HttpError;
                if (error != null)
                {
                    content = null;
                    if (error.ModelState != null)
                    {
                        var httpErrorObject = response.Content.ReadAsStringAsync().Result;
                        var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };
                        var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);
                        var modelStateValues = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value));
                        for (int i = 0; i < modelStateValues.Count(); i++)
                        {
                            modelStateErrors.Add(modelStateValues.ElementAt(i));
                        }
                    }
                    else
                    {
                        foreach (var item in error)
                        {
                            modelStateErrors.Add(item.Value.ToString());
                        }
                    }
                }
            }
            int statuscode = (int)response.StatusCode;
            var newResponse = request.CreateResponse(response.StatusCode, new ResponseResult(modelStateErrors, content , statuscode, response.StatusCode.ToString()));
            foreach (var header in response.Headers)
            {
                newResponse.Headers.Add(header.Key, header.Value);
            }

            return newResponse;
        }
    }
}