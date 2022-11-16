using SuggestionAppLibrary;
using SuggestionAppUI;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    var db = services.GetRequiredService<IDbConnection>();
    await DataSeed.SeedAsync(db);
}
catch (Exception ex)
{
    if (app.Environment.IsDevelopment())
    {
        logger.LogError("Error seeding data", ex);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
