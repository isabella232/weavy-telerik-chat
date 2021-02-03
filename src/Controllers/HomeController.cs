using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace WeavyTelerikChat.Controllers {
    
    public class HomeController : Controller {

        [Route("")]
        public ActionResult Index() {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        [Authorize]
        [Route("chat")]
        public ActionResult Chat() {    
            return View();
        }
    }
}
