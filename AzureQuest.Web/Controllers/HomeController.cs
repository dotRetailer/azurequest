using AzureQuest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Security.Claims;

namespace AzureQuest.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache) { }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            //Just to trigger an error when the user is not authenticated to demonstrate the insights of the exception
            ViewData["MyUserId"] = User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
