// **AI** i need to generate 100 synthetic data(classes) and connect them into the database

// **AI** i want to add an authentication to this razor page project. 
//follow these steps (steps provided in the lecture notes) and integrate authentication into my codes. 
//make sure already-existing functionalities remain working:

// **AI** i applied the steps until the step 5, but i dont want to use login partial because i already have my login page.
// if you need any of my codes, i can share it with you if you provide me the file names.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToFolder("/Identity/Account");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Logout");
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<SchoolDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Error";
    options.LogoutPath = "/Logout";

    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Allows HTTP in development
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SchoolDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Create admin role
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create admin user
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create user role
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Create regular user
        var userEmail = "user@example.com";
        var regularUser = await userManager.FindByEmailAsync(userEmail);
        if (regularUser == null)
        {
            regularUser = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                FirstName = "Regular",
                LastName = "User"
            };

            await userManager.CreateAsync(regularUser, "User123!");
        }

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
                    IsActive = true,
                    UserId = adminUser.Id
                });
            }

            context.Classes.AddRange(classes);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages().WithStaticAssets();
app.Run();