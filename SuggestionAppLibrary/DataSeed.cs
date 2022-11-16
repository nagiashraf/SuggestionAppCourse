using MongoDB.Driver;
using SuggestionAppLibrary.DataAccess;

namespace SuggestionAppLibrary;
public class DataSeed
{
    public async static Task SeedAsync(IDbConnection db)
    {
        await SeedCategories(db);
        await SeedStatuses(db);
        await SeedUsers(db);
    }
    public async static Task SeedCategories(IDbConnection db)
    {
        if (!(await db.CategoryCollection?.FindAsync(_ => true)).ToList().Any())
        {
            List<CategoryModel> categories = new()
            {
                new CategoryModel
                {
                    Name = "Courses",
                    Description = "Full paid courses."
                },
                new CategoryModel
                {
                    Name = "Dev Questions",
                    Description = "Advice on being a developer."
                },
                new CategoryModel
                {
                    Name = "In-Depth Tutorial",
                    Description = "A deep-dive video on how to use a topic."
                },
                new CategoryModel
                {
                    Name = "10-Minute Trainging",
                    Description = "A quick \"How do I use this?\" video."
                },
                new CategoryModel
                {
                    Name = "Other",
                    Description = "Not sure which category this fits in."
                }
            };
            await db.CategoryCollection.InsertManyAsync(categories);
        }
    }

    public async static Task SeedStatuses(IDbConnection db)
    {
        if (!(await db.StatusCollection?.FindAsync(_ => true)).ToList().Any())
        {
            List<StatusModel> statuses = new()
            {
                new StatusModel
                {
                    Name = "Completed",
                    Description = "The suggestion was accepted and the corresponding item was created."
                },
                new StatusModel
                {
                    Name = "Watching",
                    Description = "The suggestion is intersting. We are watching to see how much interest there is in it."
                },
                new StatusModel
                {
                    Name = "Upcoming",
                    Description = "The suggestion was accepted and it will be released soon."
                },
                new StatusModel
                {
                    Name = "Dismissed",
                    Description = "The suggestion was not something that we are going to undertake."
                }
            };
            await db.StatusCollection.InsertManyAsync(statuses);
        }
    }

    public async static Task SeedUsers(IDbConnection db)
    {
        if (!(await db.UserCollection?.FindAsync(_ => true)).ToList().Any())
        {
            List<UserModel> users = new()
            {
                new UserModel
                {
                    FirstName = "Tim",
                    LastName = "Corey",
                    DisplayName = "Timmy",
                    Email = "tim@test.com"
                },
                new UserModel
                {
                    FirstName = "John",
                    LastName = "Doe",
                    DisplayName = "Johnny",
                    Email = "john@test.com"
                },
                new UserModel
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    DisplayName = "Jenny",
                    Email = "jane@test.com"
                }
            };
            await db.UserCollection.InsertManyAsync(users);
        }
    }
}
