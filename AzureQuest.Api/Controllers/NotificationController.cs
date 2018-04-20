using AzureQuest.Api.Repositories.Contracts;
using AzureQuest.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureQuest.Api.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class NotificationController : Controller
    {
        private INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpPost("{id}"), Produces("application/json", Type = typeof(OperationResult))]
        public async Task<IActionResult> Post(string id)
        {
            var result = await _notificationRepository.SendNotificationEmail(id);
            return Ok(result);
        }
        
    }
}
