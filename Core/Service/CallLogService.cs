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

    

        public async Task<(List<ApiCallLog> Items, int TotalCount)> GetApiCallLogsAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.CallLogs
                .OrderByDescending(e => e.DateTime);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
} 