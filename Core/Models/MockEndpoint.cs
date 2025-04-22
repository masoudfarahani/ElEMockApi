namespace ELE.MockApi.Core.Models
{
    public class MockEndpoint
    {
        private MockEndpoint() { }

        public MockEndpoint(AvailableHttpMethods httpMethod, string baseUrl, List<AvailableResponse> responses)
        {
            HttpMethod = httpMethod;
            BaseUrl = baseUrl;
            Responses = responses;
        }

        public Guid Id { get; set; }
        public AvailableHttpMethods HttpMethod { get; set; }
        public string BaseUrl { get; set; } = "";
        public string? Rule { get; set; }
        public List<AvailableResponse> Responses { get; set; } = [];
    }

    public class ApiCallLog
    {
        private ApiCallLog()
        { }

        public ApiCallLog(string url, string method, string status, string headers)
        {
            Id = Guid.NewGuid();
            this.Url = url;
            this.Method = method;
            this.Status = status;
            DateTime = DateTime.Now;
            Headers = headers;
        }
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string? Body { get; set; }
        public string Headers { get; set; }
        public string? Response { get; set; }
        public string Status { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class Log
    {
        private Log() { }

        public Log(string content)
        {
            Content = content;
            Id = Guid.NewGuid();
            DateTime = DateTime.Now;
        }

        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
    }
}
