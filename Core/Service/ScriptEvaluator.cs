using ELE.MockApi.Core.Models;
using Jint;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ELE.MockApi.Core.Service
{


    public class ScriptEvaluator
    {
        private const string nameTokens = "abcvefghijklmnopqrstuvwxyz";
        public LogService _logService { get; private set; }
        public Engine Engine { get; private set; }
        private static Random rnd = new Random();
        public ScriptEvaluator(LogService logger)
        {
            _logService = logger;
            Engine = new Engine().SetValue("console", new
            {
                log = new Action<object>( c => 
            _logService.Add(new Log(c?.ToString()??"-") { LogType = LogType.UserLog }).Wait())
            });
        }

        public void SetRequestBody(object? requestBody)
        {
            if (requestBody is null)
            {
                requestBody = new object();
            }

            var requestBodyJson = JsonSerializer.Serialize(requestBody);

            Engine.Execute($"var {KeywordsReplacements.RequestBody} = JSON.parse('{requestBodyJson}');");
        }

        public void SetRule(string? rule)
        {
            if (string.IsNullOrEmpty(rule))
                return;

            var rulesb = new StringBuilder(rule);
            rulesb = GetClearedString(rulesb);

            Engine.Execute(rulesb.ToString());
        }

        public void SetRequestHeaders(IHeaderDictionary headers)
        {
            if (headers.Any())
            {
                var headerJson = JsonSerializer.Serialize(headers);

                Engine.SetValue(KeywordsReplacements.RequestHeaders, headerJson);
            }
        }

        public void SetRequestQueryStrings(IQueryCollection queries)
        {
            if (queries.Any())
            {
                var queriesJson = JsonSerializer.Serialize(queries);

                Engine.SetValue(KeywordsReplacements.QueryStrings, queriesJson);
            }
        }

        public string? PrepareResponseBody(string? responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return null;

            var placeHolders = GetPlaceHolders(responseBody);
            var scipts = GetScripts(responseBody);
            var result = new StringBuilder(responseBody);

            foreach (var sci in scipts)
            {
                Evaluate(sci.matchedValue, sci.placeHolderContent, result);
            }

            foreach (var sci in placeHolders)
            {
                Evaluate(sci.matchedValue, sci.placeHolderContent, result);
            }

            return result.ToString();
        }

        private List<(string matchedValue, string placeHolderContent)> GetPlaceHolders(string body)
        {
            var pattern = @"\[\$(?!=>)(.*?)\$\]";

            var regex = new Regex(pattern);

            var matches = Regex.Matches(body, pattern).Select(c => (c.Value, c.Groups[1].Value)).ToList();

            return matches;
        }


        private List<(string matchedValue, string placeHolderContent)> GetScripts(string body)
        {
            var pattern = @"\[\$=>(.*?)\$\]";

            var regex = new Regex(pattern);

            var matches = Regex.Matches(body, pattern).Select(c => (c.Value, c.Groups[1].Value)).ToList();
            return matches;
        }

        private void Evaluate(string script, string placeHolderContent, StringBuilder response)
        {
            var sb = GetClearedString(new StringBuilder(placeHolderContent));

            var randomName = GetRandomName();

            var result = Engine.Execute($"var {randomName} = {sb.ToString()}").GetValue(randomName).ToString();

            response.Replace(script, result);
        }

        private StringBuilder GetClearedString(StringBuilder sb)
        {
            sb = sb
                .Replace(Keywords.RequestBody, KeywordsReplacements.RequestBody)
                .Replace(Keywords.QueryStrings, KeywordsReplacements.QueryStrings)
                .Replace(Keywords.RequestHeaders, KeywordsReplacements.RequestHeaders);

            return sb;
        }

        private string GetRandomName()
        {
            var isdefined = true;
            var result = "";

            while (isdefined)
            {
                var tmpNameTokens = Enumerable.Range(0, 7).Select(c => nameTokens[rnd.Next(0, nameTokens.Length)]);
                result = string.Join("", tmpNameTokens);
                isdefined = Engine.Evaluate($"typeof {result} !== 'undefined'").AsBoolean();
            }

            return result;
        }

        internal int GetRuleResult()
        {
            var result = Engine.Evaluate("evaluate()").AsNumber();
            return Convert.ToInt32(result);
        }
    }
}
