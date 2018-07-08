using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Auction.Startup))]

// assembly for signalR
[assembly: OwinStartup(typeof(Auction.Startup))]

namespace Auction
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // for signalR
            app.MapSignalR();
        }
    }
}
