using AzureQuest.Common;
using AzureQuest.Web.Models.TaskViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureQuest.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected IMemoryCache _cache;
        protected string MyAPIUrl { get; }
        protected const string ApiStateCacheKey = "last-api-state";

        public BaseController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this._configuration = configuration;
            this._cache = memoryCache;
            this.MyAPIUrl = _configuration.GetConnectionString("MyAPIAddress");
        }

        public bool? IsApiAvaliable
        {
            get
            {
                return _cache.GetOrCreate(ApiStateCacheKey, entry =>
                 {
                     entry.SlidingExpiration = TimeSpan.FromSeconds(3);

                     bool? lastvalue = null;
                     try
                     {
                         var result = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/state", Method.GET);
                         if (result.Success) { lastvalue = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationResult>(result.Message).Success; }
                     }
                     catch (Exception) { lastvalue = false; }
                     return lastvalue;
                 });
            }
        }

        public SimpleUser CurrentUser
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new SimpleUser()
                    {
                        Name = User.Identity.Name,
                        Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity.Name,
                        Id = User.FindFirst(ClaimTypes.NameIdentifier).Value
                    };
                }
                return null;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.IsApiAvaliable = this.IsApiAvaliable;
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.IsApiAvaliable = this.IsApiAvaliable;
            return base.OnActionExecutionAsync(context, next);
        }

    }
}
