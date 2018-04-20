using AzureQuest.Common;
using AzureQuest.Web.Models.TaskViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;

namespace AzureQuest.Web.Controllers
{
    [Authorize]
    public class TaskController : BaseController
    {
        public TaskController(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache) { }

        public IActionResult Index()
        {
            IEnumerable<SimpleTask> model = new List<SimpleTask>();

            var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task", Method.GET);
            if (request.Success && !string.IsNullOrEmpty(request.Message))
            {
                model = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationResultList<SimpleTask>>(request.Message)?.Data;
            }

            return View(model);
        }

        public IActionResult Details(string id)
        {
            SimpleTask model = GetTask(id);
            if (model != null) return View(model); else return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind] SimpleTask task)
        {
            if (ModelState.IsValid)
            {
                task.User = CurrentUser;
                var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task/", Method.POST, task);
                if (!request.Success && !string.IsNullOrEmpty(request.Message)) { ModelState.AddModelError(string.Empty, request.Message); }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(task);
        }

        public IActionResult Edit(string id)
        {
            SimpleTask model = GetTask(id);
            if (model != null) return View(model); else return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind] SimpleTask task)
        {
            if (ModelState.IsValid)
            {
                task.User = CurrentUser;
                var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task/", Method.POST, task);
                if (!request.Success && !string.IsNullOrEmpty(request.Message)) { ModelState.AddModelError(string.Empty, request.Message); }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(task);
        }

        public IActionResult Delete(string id)
        {
            SimpleTask model = GetTask(id);
            if (model != null) return View(model); else return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([Bind] SimpleTask task)
        {
            var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task/{task.Id}", Method.DELETE);
            if (!request.Success && !string.IsNullOrEmpty(request.Message)) { ModelState.AddModelError(string.Empty, request.Message); return View(task); }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cancel(string id)
        {
            SimpleTask model = GetTask(id);
            if (model != null) return View(model); else return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel([Bind] SimpleTask task)
        {
            var status = new SimpleTaskStatus()
            {
                Id = Guid.NewGuid().ToString(),
                TaskId = task.Id,
                RegistrationDate = DateTime.Now,
                RegistrationUser = CurrentUser,
                Status = TaskStatusType.Canceled
            };

            var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task/{task.Id}", Method.PUT, status);
            if (!request.Success && !string.IsNullOrEmpty(request.Message)) { ModelState.AddModelError(string.Empty, request.Message); return View(task); }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Complete(string id)
        {
            SimpleTask model = GetTask(id);
            if (model != null) return View(model); else return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete([Bind] SimpleTask task)
        {
            var status = new SimpleTaskStatus()
            {
                Id = Guid.NewGuid().ToString(),
                TaskId = task.Id,
                RegistrationDate = DateTime.Now,
                RegistrationUser = CurrentUser,
                Status = TaskStatusType.Done
            };

            var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task/{task.Id}", Method.PUT, status);
            if (!request.Success && !string.IsNullOrEmpty(request.Message)) { ModelState.AddModelError(string.Empty, request.Message); return View(task); }
            return RedirectToAction(nameof(Index));
        }

        public SimpleTask GetTask(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var request = Common.SecureRequest.RequestClient.Request($"{this.MyAPIUrl}/api/task/{id}", Method.GET);
                if (request.Success && !string.IsNullOrEmpty(request.Message))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<OperationResult<SimpleTask>>(request.Message)?.Data;
                }
            }
            return null;
        }


    }

}
