namespace SuggestionAppLibrary.DataAccess;

public interface ISuggestionData
{
    Task CreateSuggestionAsync(SuggestionModel suggestion);
    Task<List<SuggestionModel>> GetApprovedSuggestionsAsync();
    Task<SuggestionModel> GetSuggestionAsync(string id);
    Task<List<SuggestionModel>> GetSuggestionsAsync();
    Task<List<SuggestionModel>> GetSuggestionsPendingApprovalAsync();
    Task UpdateSuggestionAsync(SuggestionModel suggestion);
    Task UpvoteOrRemoveUpvoteSuggestionAsync(string suggestionId, string upvotingUserId);
}