using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace SuggestionAppLibrary.DataAccess;
public class MongoSuggestionData : ISuggestionData
{
    private const string SuggestionsCacheKey = "SuggestionData";
    private readonly IMongoCollection<SuggestionModel> _suggestions;
    private readonly IDbConnection _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MongoSuggestionData> _logger;
    private readonly IUserData _userData;

    public MongoSuggestionData(IDbConnection db, IMemoryCache cache, ILogger<MongoSuggestionData> logger, IUserData userData)
    {
        _suggestions = db.SuggestionCollection;
        _db = db;
        _cache = cache;
        _logger = logger;
        _userData = userData;
    }

    public async Task<List<SuggestionModel>> GetSuggestionsAsync()
    {
        if (!_cache.TryGetValue(SuggestionsCacheKey, out List<SuggestionModel> suggestions))
        {
            var suggestionsFromDb = await _suggestions.FindAsync(s => !s.IsArchived);
            suggestions = suggestionsFromDb.ToList();

            _cache.Set(SuggestionsCacheKey, suggestions, TimeSpan.FromMinutes(1));
        }

        return suggestions;
    }

    public async Task<List<SuggestionModel>> GetApprovedSuggestionsAsync()
    {
        var suggestions = await GetSuggestionsAsync();
        return suggestions.Where(s => s.IsApprovedForRelease).ToList();
    }

    public async Task<List<SuggestionModel>> GetSuggestionsPendingApprovalAsync()
    {
        var suggestions = await GetSuggestionsAsync();
        return suggestions.Where(s => !s.IsApprovedForRelease && !s.IsRejected).ToList();
    }

    public async Task<SuggestionModel> GetSuggestionAsync(string id)
    {
        var suggestion = await _suggestions.FindAsync(s => s.Id == id);
        return suggestion.FirstOrDefault();
    }

    public async Task UpdateSuggestionAsync(SuggestionModel suggestion)
    {
        await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
        _cache.Remove(SuggestionsCacheKey);
    }

    public async Task UpvoteOrRemoveUpvoteSuggestionAsync(string suggestionId, string upvotingUserId)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var suggestion = (await _suggestions.FindAsync(s => s.Id == suggestionId)).First();
            var user = await _userData.GetUserAsync(upvotingUserId);

            if (suggestion.Author.Id != upvotingUserId)
            {
                bool FirstTimeUpvoted = suggestion.UserVotes.Add(upvotingUserId);

                if (!FirstTimeUpvoted)
                {
                    suggestion.UserVotes.Remove(upvotingUserId);
                }

                await UpdateSuggestionAsync(suggestion);

                if (FirstTimeUpvoted)
                {
                    user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
                }
                else
                {
                    var suggestionToRemove = user.VotedOnSuggestions.First(s => s.Id == suggestionId);
                    user.VotedOnSuggestions.Remove(suggestionToRemove);
                }

                await _userData.UpdateUserAsync(user);

                await session.CommitTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error upvoting suggestion", ex);
            await session.AbortTransactionAsync();
        }
    }

    public async Task CreateSuggestionAsync(SuggestionModel suggestion)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            await _suggestions.InsertOneAsync(suggestion);

            var author = await _userData.GetUserAsync(suggestion.Author.Id);
            author.AuthorSuggestions.Add(new BasicSuggestionModel(suggestion));
            await _userData.UpdateUserAsync(author);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating suggestion", ex);
            await session.AbortTransactionAsync();
        }
    }
}
