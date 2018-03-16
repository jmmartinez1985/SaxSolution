using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace Banistmo.Sax.WebApi.Filters
{
    public class ErrorLoggingFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            log.Info("------------------------------BEGIN ERROR FOUND--------------------------------" + Environment.NewLine);
            log.Info("Controller : " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine);
            log.Info("Action: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName + Environment.NewLine);
            log.Info("JSON: " + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments) + Environment.NewLine);
            log.Info("Error Message: " + JsonConvert.SerializeObject(actionExecutedContext.Exception.Message) + Environment.NewLine);
            log.Info("Error Source: " + JsonConvert.SerializeObject(actionExecutedContext.Exception.Source) + Environment.NewLine);
            log.Info("Error StackTrace: " + JsonConvert.SerializeObject(actionExecutedContext.Exception.StackTrace) + Environment.NewLine);
            log.Info("------------------------------END ERROR FOUND--------------------------------" + Environment.NewLine);
            base.OnException(actionExecutedContext);
        }
    }
}