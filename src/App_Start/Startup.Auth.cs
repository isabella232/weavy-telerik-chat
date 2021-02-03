using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace WeavyTelerikChat {
    public partial class Startup {

        public void ConfigureAuth(IAppBuilder app) {

            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/sign-in"),
                Provider = new CookieAuthenticationProvider {
                }
            });
        }
    }
}