using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;
public class MongoStatusData : IStatusData
{
    private const string StatusesCacheKey = "StatusData";
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache _cache;

    public MongoStatusData(IDbConnection db, IMemoryCache cache)
    {
        _statuses = db.StatusCollection;
        _cache = cache;
    }

    public async Task<List<StatusModel>> GetStatusesAsync()
    {
        if (!_cache.TryGetValue(StatusesCacheKey, out List<StatusModel> statuses))
        {
            var statusesFromDb = await _statuses.FindAsync(_ => true);
            statuses = statusesFromDb.ToList();

            _cache.Set(StatusesCacheKey, statuses, TimeSpan.FromDays(1));
        }

        return statuses;
    }

    public Task CreateStatusAsync(StatusModel status)
    {
        return _statuses.InsertOneAsync(status);
    }

    public Task RemoveStatusAsync(string id)
    {
        return _statuses.DeleteOneAsync(c => c.Id == id);
    }
}
