using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreWebApp.Models;

namespace NetCoreWebApp.Controllers
{
    public class HomeController : Controller
    {

        //1.构造函数注入
        IOptions<IndexSetting> _ser;
        public HomeController(IOptions<IndexSetting> ser)
        {
            this._ser = ser;
        }
        public IActionResult Index2([FromServices]IOptions<IndexSetting> ser/*2.FromServices方式注入*/)
        {
            //2.这种方式比较适合，service不是在整个controller里面使用的情况下。只有某个单独的action需要。可以考虑使用这种方式
            _ser = ser;

            //3.IServiceProvider。ioc容器直接获取。
            //3.官方并不推荐使用，官方原话
            //Generally, you shouldn’t use these properties directly, 
            //preferring instead to request the types your classes you require via your class’s constructor, 
            //and letting the framework inject these dependencies. 
            //This yields classes that are easier to test (see Testing) and are more loosely coupled.    
            //大意是不应该使用这种方式，而应该使用构造函数的方式注入。因为能够方便测试并且设计上更加解耦。          
            _ser = this.HttpContext.RequestServices.GetService<IOptions<IndexSetting>>();
            _ser.Value.Desc = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            return View(ser.Value);
        }

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
