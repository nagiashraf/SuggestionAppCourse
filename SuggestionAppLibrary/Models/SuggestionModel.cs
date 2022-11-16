namespace SuggestionAppLibrary.Models;

public class SuggestionModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public CategoryModel Category { get; set; }
    public BasicUserModel Author { get; set; }
    public StatusModel Status { get; set; }
    public HashSet<string> UserVotes { get; set; } = new();
    public string AdminNotes { get; set; }
    public bool IsApprovedForRelease { get; set; }
    public bool IsArchived { get; set; }
    public bool IsRejected { get; set; }
}
