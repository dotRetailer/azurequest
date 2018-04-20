using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharpRaven;

namespace AzureQuest.Web
{
    public sealed class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.
                    CreateLogger<CustomExceptionHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //var ravenClient = new RavenClient("{MySentryIoServerSideDSNKey}");
                //await ravenClient.CaptureAsync(new SharpRaven.Data.SentryEvent(ex));
                throw;
            }
        }
    }
}
