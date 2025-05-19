using ELE.MockApi.Core.Db;
using ELE.MockApi.Core.FormModels;
using ELE.MockApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ELE.MockApi.Core.Service
{
    public class CallLogService
    {
        private readonly DataBaseContext _dbContext;

        public CallLogService(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(ApiCallLog model)
        {
            _dbContext.CallLogs.Add(model);
            await _dbContext.SaveChangesAsync();
        }


        public async Task Clear()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [CallLogs]");
        }
        public async Task<(List<ApiCallLog> Items, int TotalCount)> GetApiCallLogsAsync(int pageNumber, int pageSize, string urlFilter = "")
        {
            var query = _dbContext.CallLogs.AsQueryable();

            if(!string.IsNullOrWhiteSpace(urlFilter))
                query=query.Where(c=>c.Url.Contains(urlFilter));

            query = query.OrderByDescending(e => e.DateTime);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}