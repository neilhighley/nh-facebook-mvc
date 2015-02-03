using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(neilhighley_fb.Startup))]
namespace neilhighley_fb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
