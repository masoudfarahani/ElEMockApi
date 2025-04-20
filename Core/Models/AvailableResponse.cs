using ELE.MockApi.Core.FormModels;
using System.Net;

namespace ELE.MockApi.Core.Models
{
    public class AvailableResponse
    {
        public Guid Id { get; set; }
        public HttpStatus Status { get; set; }
        public string Body { get; set; }
    }
}
