using Autofac;
using Autofac.Integration.WebApi;
using Banistmo.Sax.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.App_Start
{
    public static class AutofacWebApiConfig
    {
        public static void DependencyBuilder()
        {
            // Create the builder with which components/services are registered.
            var builder = new ContainerBuilder();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                                      .Where(t => t.GetCustomAttribute<InjectableAttribute>() != null)
                                      .AsImplementedInterfaces()
                                      .InstancePerRequest();

            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            //Build the Container
            var container = builder.Build();

            //Create the Dependency Resolver
            var resolver = new AutofacWebApiDependencyResolver(container);

            //COnfiguring WebApi with Dependency Resolver
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

        }
    }
}