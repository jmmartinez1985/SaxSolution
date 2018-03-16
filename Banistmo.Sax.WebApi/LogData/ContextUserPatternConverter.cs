using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Banistmo.Sax.WebApi.LogData
{
    public class ContextUserPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, LoggingEvent loggingEvent)
        {
            var userName = string.Empty;
            var context = HttpContext.Current;
            if (context != null && context.User != null && context.User.Identity.IsAuthenticated)
            {
                userName = context.User.Identity.Name;
            }
            else
            {
                var threadPincipal = Thread.CurrentPrincipal;
                if (threadPincipal != null && threadPincipal.Identity.IsAuthenticated)
                {
                    userName = threadPincipal.Identity.Name;
                }
            }
            if (string.IsNullOrEmpty(userName))
            {
                userName = loggingEvent.UserName;
            }
            writer.Write(userName);
        }
    }
}