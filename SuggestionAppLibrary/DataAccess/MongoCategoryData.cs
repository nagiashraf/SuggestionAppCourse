using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;
public class MongoCategoryData : ICategoryData
{
    private const string CategoriesCacheKey = "CategoryData";
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<CategoryModel> _categories;

    public MongoCategoryData(IDbConnection db, IMemoryCache cache)
    {
        _categories = db.CategoryCollection;
        _cache = cache;
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        if (!_cache.TryGetValue(CategoriesCacheKey, out List<CategoryModel> categories))
        {
            var categoriesFromDb = await _categories.FindAsync(_ => true);
            categories = categoriesFromDb.ToList();

            _cache.Set(CategoriesCacheKey, categories, TimeSpan.FromDays(1));
        }

        return categories;
    }

    public Task CreateCategoryAsync(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }

    public Task RemoveCategoryAsync(string id)
    {
        return _categories.DeleteOneAsync(c => c.Id == id);
    }
}
