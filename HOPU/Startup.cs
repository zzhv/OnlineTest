using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HOPU.Startup))]
namespace HOPU
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
