using Microsoft.AspNetCore.Authentication.Cookies;
using KovsieAssetTracker.Data;
using KovsieAssetTracker.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using QuestPDF;

var builder = WebApplication.CreateBuilder(args);

// Add services to container
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    // default policy is fine; we will decorate controllers/actions with roles
});
// Add repositories (we’ll register them after we create them)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerificationRepository, VerificationRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});



QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

// Serve static files (for images)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

app.MapControllers();
app.UseCors("AllowAll");


// Seed test users (optional)
using (var scope = app.Services.CreateScope())
{
    SeedData.EnsureSeedData(scope);
}

app.Run();
