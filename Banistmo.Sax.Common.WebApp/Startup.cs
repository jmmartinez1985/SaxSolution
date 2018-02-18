using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Banistmo.Sax.Common.WebApp.Startup))]
namespace Banistmo.Sax.Common.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
