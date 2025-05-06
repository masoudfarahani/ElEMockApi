using Microsoft.AspNetCore.Mvc.Filters;

namespace ELE.MockApi.Controllers.Filters
{
    public class ContentTypeFilter : IResultFilter
    {

        public ContentTypeFilter()
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // تغییر Content-Type قبل از اجرای نتیجه
            context.HttpContext.Response.ContentType = "application/json";
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // عملیات پس از اجرای نتیجه
        }
    }
}
