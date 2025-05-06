using ELE.MockApi.Core.Db;
using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ELE.MockApi.Core.Service
{
    public class EndpointService
    {
        private readonly DataBaseContext _dbContext;

        public EndpointService(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MockEndpoint> CreateEndpointAsync(CreateOrUpdateEndpointModel model)
        {
            await ValidateEndpointUniquenessAsync(model.BaseUrl, model.HttpMethod);
            ValidateResponseStatusCodes(model.Responses);

            var responses = model.Responses.Select(r => new AvailableResponse
            {
                Id = Guid.NewGuid(),
                Status = r.Status,
                Body = r.Body
            }).ToList();

            var endpoint = new MockEndpoint(model.HttpMethod, model.BaseUrl, responses)
            {
                Id = Guid.NewGuid(),
                Rule = model.Rule
            };

            _dbContext.Endpoints.Add(endpoint);
            await _dbContext.SaveChangesAsync();

            return endpoint;
        }

        public async Task<MockEndpoint> UpdateEndpointAsync(CreateOrUpdateEndpointModel model)
        {
            var existingEndpoint = await _dbContext.Endpoints
                .Include(e => e.Responses)
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (existingEndpoint == null)
            {
                throw new InvalidOperationException($"Endpoint with ID '{model.Id}' not found.");
            }

            // Check if the base URL or HTTP method is being changed
            if (existingEndpoint.BaseUrl != model.BaseUrl || existingEndpoint.HttpMethod != model.HttpMethod)
            {
                await ValidateEndpointUniquenessAsync(model.BaseUrl, model.HttpMethod);
            }

            ValidateResponseStatusCodes(model.Responses);

            // Update the endpoint properties
            existingEndpoint.HttpMethod = model.HttpMethod;
            existingEndpoint.BaseUrl = model.BaseUrl;
            existingEndpoint.Rule = model.Rule;

            // Clear existing responses
            existingEndpoint.Responses.Clear();

            // Add new responses
            var newResponses = model.Responses.Select(r => new AvailableResponse
            {
                Id = r.Id == Guid.Empty ? Guid.NewGuid() : r.Id,
                Status = r.Status,
                Body = r.Body
            }).ToList();

            existingEndpoint.Responses.AddRange(newResponses);

            await _dbContext.SaveChangesAsync();

            return existingEndpoint;
        }

        public async Task<(List<MockEndpoint> Items, int TotalCount)> GetEndpointsAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Endpoints
                .Include(e => e.Responses)
                .OrderByDescending(e => e.BaseUrl);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private async Task ValidateEndpointUniquenessAsync(string baseUrl, AvailableHttpMethods httpMethod)
        {
            var existingEndpoint = await _dbContext.Endpoints
                .FirstOrDefaultAsync(e => e.BaseUrl == baseUrl && e.HttpMethod == httpMethod);

            if (existingEndpoint != null)
            {
                throw new InvalidOperationException($"An endpoint with BaseUrl '{baseUrl}' and HTTP method '{httpMethod}' already exists.");
            }
        }

        private void ValidateResponseStatusCodes(List<CreateOrUpdateResponseModel> responses)
        {
            var duplicateStatusCodes = responses
                .GroupBy(r => r.Status)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateStatusCodes.Any())
            {
                throw new InvalidOperationException($"Duplicate status codes found: {string.Join(", ", duplicateStatusCodes)}");
            }
        }

        public async Task DeleteEndpointAsync(Guid endpointId)
        {
            var endpoint = await _dbContext.Endpoints
                .FirstOrDefaultAsync(e => e.Id == endpointId);

            if (endpoint == null)
            {
                throw new InvalidOperationException($"Endpoint with ID '{endpointId}' not found.");
            }

            _dbContext.Endpoints.Remove(endpoint);
            await _dbContext.SaveChangesAsync();
        }
    }
} 