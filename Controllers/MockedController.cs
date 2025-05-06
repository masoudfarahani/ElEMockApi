using ELE.MockApi.Controllers.Filters;
using ELE.MockApi.Core.Db;
using ELE.MockApi.Core.Models;
using ELE.MockApi.Core.Service;
using Jint;
using Jint.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Web;
namespace ELE.MockApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MockedController : ControllerBase
    {
        private readonly DataBaseContext _context;
        private readonly CallLogService callLogService;
        private readonly LogService _logService;

        public MockedController(LogService logger, DataBaseContext context, CallLogService callLogService)
        {
            _logService = logger;
            _context = context;
            this.callLogService = callLogService;
        }

        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpPatch]
        [HttpDelete]
        [Route("{*url}")]

        public async Task<IActionResult> Action(string url, object? requestBody)
        {
            
            var requestPath = HttpUtility.UrlDecode($"/{url}").ToLower();
            var method = this.HttpContext.Request.Method.ToLower();
            var headers = this.HttpContext.Request.Headers;
            var queries = this.HttpContext.Request.Query;

            var evaluator = new ScriptEvaluator(_logService);
            evaluator.SetRequestBody(requestBody);
            evaluator.SetRequestHeaders(headers);
            evaluator.SetRequestQueryStrings(queries);

            var endpoint = await _context.Endpoints.SingleOrDefaultAsync(c => c.BaseUrl.ToLower() == requestPath && c.HttpMethod.ToString().ToLower() == method);


            if (endpoint == null)
                return NotFound(value: "can not find mocked api");


            int status = 200;

            if (endpoint.Rule != null)
            {
                evaluator.SetRule(endpoint.Rule);
                status = evaluator.GetRuleResult();
            }

            var availableResponse = endpoint.Responses.SingleOrDefault(c => c.Status.GetHashCode() == status);

            if (availableResponse == null)
            {
                return NotFound($"Correspond response not declared for api (status code {status})");
            }

            var result = evaluator.PrepareResponseBody(availableResponse.Body);

           await LogCall(this.HttpContext.Request.Path,requestBody, method, headers, status, result);

           

            if (result != null)
                return StatusCode(status, JsonSerializer.Deserialize<object>(result, new JsonSerializerOptions { WriteIndented = true }));
            else
                return StatusCode(status);
        }

        private async Task LogCall(string url, object? requestBody, string method, IHeaderDictionary headers, int status, string? result)
        {
            var log = new ApiCallLog(url, method, status.ToString(), JsonSerializer.Serialize(headers))
            {
                Body = JsonSerializer.Serialize(requestBody),
                Response = result
            };

            await callLogService.Add(log);
        }
    }
}
