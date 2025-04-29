namespace ELE.MockApi.Core.Models
{
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
}
