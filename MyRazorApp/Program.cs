// **AI** i need to generate 100 synthetic data(classes) and connect them into the database
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SchoolDbContext>();
        context.Database.EnsureCreated(); // Creates database if not exists

        if (!context.Classes.Any())
        {
            var random = new Random();
            var classPrefixes = new[] { "Math", "Science", "History", "Art", "Music", "Physics", "Chemistry", "Biology", "Literature", "Geography" };
            var classSuffixes = new[] { "101", "102", "201", "202", "301", "302", "Advanced", "Beginner", "Intermediate", "Workshop" };

            var classes = new List<Class>();
            for (int i = 0; i < 100; i++)
            {
                classes.Add(new Class
                {
                    ClassName = $"{classPrefixes[random.Next(classPrefixes.Length)]} {classSuffixes[random.Next(classSuffixes.Length)]} {random.Next(1, 5)}",
                    StudentCount = random.Next(10, 51),
                    Description = $"Description for class number {i + 1}. Focuses on core concepts and practical applications.",
                    IsActive = true
                });
            }

            context.Classes.AddRange(classes);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Rest of the middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages().WithStaticAssets();
app.Run();