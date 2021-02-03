using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeavyTelerikChat.Startup))]
namespace WeavyTelerikChat {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}