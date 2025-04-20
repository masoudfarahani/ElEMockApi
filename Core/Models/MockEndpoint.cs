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
}
