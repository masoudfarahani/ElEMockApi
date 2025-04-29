using ELE.MockApi.Core.Models;
using ELE.MockApi.Core.Service;

namespace ELE.MockApi.Controllers.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public LogService _logservice { get; set; }

        public ExceptionMiddleware(LogService logservice)
        {
            _logservice = logservice;
        }

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {


            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await _logservice.Add(new Log(ex.Message) { LogType = LogType.AppLog });
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
            }

        }
    }
}
