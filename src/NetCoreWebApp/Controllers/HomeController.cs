using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices]ILogger<HomeController> logger, [FromServices]ILoggerFactory factory)
        {
            logger.LogInformation("begin");
            using (logger.BeginScope("{0},{1}", "你是i", "SB！！"))
            {
                logger.LogError("error......................");
                logger.LogInformation("LogInformation......................");
            }
            logger.LogInformation("end");


            factory.CreateLogger("Test.NewFile").LogError("!!!!!!!!!!!!!!!!!!!!!!!!!");

            return this.Content(System.IO.Directory.GetCurrentDirectory());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
