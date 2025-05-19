using ELE.MockApi.Core.Models;

namespace ELE.MockApi.Core.FormModels
{
    public class CreateOrUpdateEndpointModel
    {
        public Guid? Id { get; set; }
        public AvailableHttpMethods HttpMethod { get; set; }
        public string BaseUrl { get;  set; } = "";
        public string? Rule { get; set; }
        public string? RequestBodySchema { get; set; }

        public List<CreateOrUpdateResponseModel> Responses { get; set; } = [];
    }

    public class CreateOrUpdateResponseModel
    {
        public Guid Id { get; set; }

        public HttpStatus Status { get; set; }
        public string? Body { get; set; }
    }

    public enum HttpStatus
    {
        OK = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        Moved = 301,
        MovedPermanently = 301,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        TooManyRequests = 429,
        InternalServerError = 500,
        BadGateway = 502,
        GatewayTimeout = 504,
    }

    public class PaginationModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
