using ELE.MockApi.Core.Db;
using ELE.MockApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ELE.MockApi.Core.Service
{
    public class LogService
    {
        private readonly DataBaseContext _dbContext;

        public LogService(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Log model)
        {
            _dbContext.Logs.Add(model);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Clear()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Logs]");
        }

        public async Task<(List<Log> Items, int TotalCount)> GetLogsAsync(int pageNumber, int pageSize, LogType? logType)
        {
            var query = _dbContext.Logs.AsQueryable();

            if (logType.HasValue)
                query = query.Where(c => c.LogType == logType);


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