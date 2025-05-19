using ELE.MockApi.Core.Models;
using ELE.MockApi.Core.Service;

namespace ELE.MockApi.Controllers.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            //_logservice = logservice;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, LogService logservice)
        {


            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await logservice.Add(new Log(ex.Message) { LogType = LogType.AppLog });
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
            }

        }
    }
}
