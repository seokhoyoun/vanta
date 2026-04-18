using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Vanta.Infrastructure.Mongo;
using Vanta.Repositories.ProjectEquipments;
using Vanta.Repositories.ProjectEquipmentModules;
using Vanta.Repositories.ProjectMembers;
using Vanta.Repositories.Projects;
using Vanta.Repositories.Users;
using Vanta.Services.AdminUsers;
using Vanta.Services.Auth;
using Vanta.Services.Projects;

var builder = WebApplication.CreateBuilder(args);

string? mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
if (string.IsNullOrWhiteSpace(mongoConnectionString))
{
    throw new InvalidOperationException("ConnectionStrings:MongoDb configuration is required.");
}

string? mongoDatabaseName = builder.Configuration["MongoDb:DatabaseName"];
if (string.IsNullOrWhiteSpace(mongoDatabaseName))
{
    throw new InvalidOperationException("MongoDb:DatabaseName configuration is required.");
}

MongoClassMapRegistration.Register();

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Setup");
    options.Conventions.AllowAnonymousToPage("/Error");
});
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    IMongoClient mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
    return mongoClient.GetDatabase(mongoDatabaseName);
});
builder.Services.AddSingleton<IMongoCollectionContext, MongoCollectionContext>();
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IProjectRepository, MongoProjectRepository>();
builder.Services.AddScoped<IProjectMemberRepository, MongoProjectMemberRepository>();
builder.Services.AddScoped<IProjectEquipmentRepository, MongoProjectEquipmentRepository>();
builder.Services.AddScoped<IProjectEquipmentModuleRepository, MongoProjectEquipmentModuleRepository>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<ILocalAccountAuthService, LocalAccountAuthService>();
builder.Services.AddSingleton<SampleProjectCatalogService>();
builder.Services.AddScoped<IProjectCatalogService, MongoProjectCatalogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    bool isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
    bool mustChangePassword = context.User.HasClaim("must_change_password", "true");
    PathString requestPath = context.Request.Path;
    bool isChangePasswordPath = requestPath.StartsWithSegments("/Account/ChangePassword");
    bool isLogoutPath = requestPath.StartsWithSegments("/Account/Logout");
    bool isStaticAsset = requestPath.StartsWithSegments("/css")
        || requestPath.StartsWithSegments("/js")
        || requestPath.StartsWithSegments("/lib")
        || requestPath.StartsWithSegments("/favicon.ico");

    if (isAuthenticated && mustChangePassword && !isChangePasswordPath && !isLogoutPath && !isStaticAsset)
    {
        context.Response.Redirect("/Account/ChangePassword");
        return;
    }

    await next();
});

app.MapRazorPages();

app.Run();
