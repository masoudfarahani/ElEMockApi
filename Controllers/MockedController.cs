using ELE.MockApi.Controllers.Filters;
using ELE.MockApi.Core.Db;
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
        private readonly ILogger<MockedController> _logger;
        private readonly DataBaseContext _context;
        public MockedController(ILogger<MockedController> logger, DataBaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpPatch]
        [HttpDelete]
        [Route("{*url}")]

        public async Task<IActionResult> Action(string url, object? requestBody)
        {
            var requestPath = HttpUtility.UrlDecode(url).ToLower();
            var method = this.HttpContext.Request.Method.ToLower();
            var headers = this.HttpContext.Request.Headers;
            var queries = this.HttpContext.Request.Query;

            var evaluator = new ScriptEvaluator(_logger);
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

            if (result != null)
                return StatusCode(status, JsonSerializer.Deserialize<object>(result, new JsonSerializerOptions { WriteIndented = true }));
            else
                return StatusCode(status);
        }
    }
}
