namespace SuggestionAppLibrary.DataAccess;
public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;

    public MongoUserData(IDbConnection db)
    {
        _users = db.UserCollection;
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await _users.FindAsync(_ => true);
        return users.ToList();
    }

    public async Task<UserModel> GetUserAsync(string id)
    {
        var user = await _users.FindAsync(u => u.Id == id);
        return user.FirstOrDefault();
    }

    public async Task<UserModel> GetUserFromAuthenticationAsync(string objectId)
    {
        var user = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
        return user.FirstOrDefault();
    }

    public Task CreateUserAsync(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUserAsync(UserModel user)
    {
        return _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public Task RemoveUserAsync(string id)
    {
        return _users.DeleteOneAsync(u => u.Id == id);
    }


}
