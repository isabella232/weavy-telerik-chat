using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WeavyTelerikChat.Models;

namespace WeavyTelerikChat.Controllers {

    /// <summary>
    /// Simple controller for signing in / out demo users.
    /// </summary>
    [RoutePrefix("account")]
    public class AccountController : Controller {

        /// <summary>
        /// JWT tokens with a ridiculous long expiration. The tokens are used to authenticate to the publicly available demo instance of Weavy.
        /// </summary>
        private Dictionary<string, string> _demoTokens = new Dictionary<string, string> {
            {"oliver", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJvbGl2ZXIiLCJuYW1lIjoiT2xpdmVyIFdpbnRlciIsImV4cCI6MjUxNjIzOTAyMiwiaXNzIjoic3RhdGljLWZvci1kZW1vIiwiY2xpZW50X2lkIjoiV2VhdnlEZW1vIiwiZGlyIjoiY2hhdC1kZW1vLWRpciIsImVtYWlsIjoib2xpdmVyLndpbnRlckBleGFtcGxlLmNvbSIsInVzZXJuYW1lIjoib2xpdmVyIn0.VuF_YzdhzSr5-tordh0QZbLmkrkL6GYkWfMtUqdQ9FM" },
            {"lilly", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJsaWxseSIsIm5hbWUiOiJMaWxseSBEaWF6IiwiZXhwIjoyNTE2MjM5MDIyLCJpc3MiOiJzdGF0aWMtZm9yLWRlbW8iLCJjbGllbnRfaWQiOiJXZWF2eURlbW8iLCJkaXIiOiJjaGF0LWRlbW8tZGlyIiwiZW1haWwiOiJsaWxseS5kaWF6QGV4YW1wbGUuY29tIiwidXNlcm5hbWUiOiJsaWxseSJ9.rQvgplTyCAfJYYYPKxVgPX0JTswls9GZppUwYMxRMY0" },
            {"samara", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzYW1hcmEiLCJuYW1lIjoiU2FtYXJhIEthdXIiLCJleHAiOjI1MTYyMzkwMjIsImlzcyI6InN0YXRpYy1mb3ItZGVtbyIsImNsaWVudF9pZCI6IldlYXZ5RGVtbyIsImRpciI6ImNoYXQtZGVtby1kaXIiLCJlbWFpbCI6InNhbWFyYS5rYXVyQGV4YW1wbGUuY29tIiwidXNlcm5hbWUiOiJzYW1hcmEifQ.UKLmVTsyN779VY9JLTLvpVDLc32Coem_0evAkzG47kM" },
            {"adam", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZGFtIiwibmFtZSI6IkFkYW0gTWVyY2VyIiwiZXhwIjoyNTE2MjM5MDIyLCJpc3MiOiJzdGF0aWMtZm9yLWRlbW8iLCJjbGllbnRfaWQiOiJXZWF2eURlbW8iLCJkaXIiOiJjaGF0LWRlbW8tZGlyIiwiZW1haWwiOiJhZGFtLm1lcmNlckBleGFtcGxlLmNvbSIsInVzZXJuYW1lIjoiYWRhbSJ9.c4P-jeQko3F_-N4Ou0JQQREePQ602tNDhO1wYKBhjX8" }
        };

        /// <summary>
        /// Returns the sign in view.
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sign-in")]
        public ActionResult SignIn(string ReturnUrl) {
            var model = new SignInModel() { Path = ReturnUrl };
            return View(model);
        }

        /// <summary>
        /// Signs in the one of four demo users.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign-in-demo")]
        public ActionResult SignInDemo(SignInDemo model) {
            if (ModelState.IsValid) {
                var claims = new List<Claim>() {
                    new Claim("Token", _demoTokens[model.Username]),
                    new Claim("Name", model.Username),
                    new Claim(ClaimTypes.NameIdentifier, model.Username)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier, ClaimsIdentity.DefaultRoleClaimType);

                var authProperties = new AuthenticationProperties {
                    IsPersistent = true,
                };

                Request.GetOwinContext().Authentication.SignIn(authProperties, claimsIdentity);
                return Redirect(model.Path ?? "~/");
            } else {
                return View(model);
            }
        }

        /// <summary>
        /// Signs in the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign-in")]
        public ActionResult SignIn(SignInModel model) {

            if (ModelState.IsValid) {

                var claims = new List<Claim>() {
                    new Claim("Email", model.Email),
                    new Claim("Name", model.Name),
                    new Claim(ClaimTypes.NameIdentifier, model.Email)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier, ClaimsIdentity.DefaultRoleClaimType);

                var authProperties = new AuthenticationProperties {
                    IsPersistent = true,
                };

                Request.GetOwinContext().Authentication.SignIn(authProperties, claimsIdentity);
                return Redirect(model.Path ?? "~/");
            } else {
                return View(model);
            }
        }

        /// <summary>
        /// Generates a JWT token to authenticate against the Weavy instance.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("token")]
        public ContentResult Token() {
            var currentUser = (ClaimsIdentity)User.Identity;
            string token;
            
            if (currentUser.Claims.Any(x=>x.Type == "Token")) {
                // use the static tokens for the public demo instance
                token = currentUser.Claims.FirstOrDefault(x => x.Type == "Token").Value;
            } else {
                // generate a "proper" JWT token against your own Weavy instance
                token = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(ConfigurationManager.AppSettings["weavy-client-secret"])
                    .AddClaim("exp", DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds())
                    .AddClaim("iss", ConfigurationManager.AppSettings["weavy-client-id"])
                    .AddClaim("sub", currentUser.Claims.FirstOrDefault(x => x.Type == "Email").Value)
                    .AddClaim("email", currentUser.Claims.FirstOrDefault(x => x.Type == "Email").Value)
                    .AddClaim("name", currentUser.Claims.FirstOrDefault(x => x.Type == "Name").Value)
                .Encode();
            }
            return Content(token, "text/html");
        }

        [HttpGet]
        [Route("sign-out")]
        public ActionResult SignOut() {
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect("~/");
        }
    }
}
