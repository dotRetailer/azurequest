using AzureQuest.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureQuest.Api.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class StateController : Controller
    {
        [HttpGet, Produces("application/json", Type = typeof(OperationResult))]
        public IActionResult Get()
        {
            var result = new OperationResult(true);
            return Ok(result);
        }

    }
}
