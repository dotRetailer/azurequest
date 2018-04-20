using AzureQuest.Api.Model;
using AzureQuest.Api.Repositories.Contracts;
using AzureQuest.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureQuest.Api.Controllers
{
    [Route("api/[controller]")/*, Authorize*/]
    public class TaskController : Controller
    {
        private ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet, Produces("application/json", Type = typeof(OperationResultList<SimpleTask>))]
        public IActionResult Get([FromQuery]SimpleTaskQuery query)
        {
            var result = _taskRepository.SearchTasks(query);
            return Ok(result);
        }

        [HttpGet("{id}"), Produces("application/json", Type = typeof(OperationResult<SimpleTask>))]
        public IActionResult Get(string id)
        {
            var result = _taskRepository.GetTask(id);
            return Ok(result);
        }

        [HttpPost, Produces("application/json", Type = typeof(OperationResult))]
        public async Task<IActionResult> Post([FromBody]SimpleTask record)
        {
            var result = await _taskRepository.SaveTaskAsync(record);
            return Ok(result);
        }

        [HttpPut("{id}"), Produces("application/json", Type = typeof(OperationResult))]
        public async Task<IActionResult> Put(string id, [FromBody]SimpleTaskStatus record)
        {
            var result = await _taskRepository.UpdateTaskStatusAsync(record);
            return Ok(result);
        }

        [HttpDelete("{id}"), Produces("application/json", Type = typeof(OperationResult))]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _taskRepository.RemoveTaskAsync(id);
            return Ok(result);
        }
    }
}
