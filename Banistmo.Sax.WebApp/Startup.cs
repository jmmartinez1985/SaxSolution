using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Banistmo.Sax.WebApp.Startup))]
namespace Banistmo.Sax.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
