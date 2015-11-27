using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bliki.Startup))]
namespace Bliki
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
