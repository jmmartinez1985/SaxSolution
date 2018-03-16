using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Banistmo.Sax.WebApi.Filters
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {

        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            log.Info("Controller : " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine);
            log.Info("Action: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName + Environment.NewLine);
            log.Info("JSON: " + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments) + Environment.NewLine);
            base.OnActionExecuted(actionExecutedContext);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
        }
    }
}