using BookClient.Services.AuthServices;
using BookClient.Services.UserServices;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Add Data Protection with persistent key storage
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionKeys")))
    .SetApplicationName("BookClient")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ✅ Added for better security
    options.Cookie.SameSite = SameSiteMode.Lax; // ✅ Added for better security
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

string apiGatewayUrl = builder.Configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7000";

// Validate and ensure the URL is properly formatted
if (string.IsNullOrWhiteSpace(apiGatewayUrl))
{
    throw new InvalidOperationException("ApiGateway:BaseUrl configuration is missing or empty.");
}

if (!Uri.IsWellFormedUriString(apiGatewayUrl, UriKind.Absolute))
{
    throw new InvalidOperationException($"ApiGateway:BaseUrl configuration '{apiGatewayUrl}' is not a valid absolute URI.");
}

// Configure HttpClient for AuthService
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ✅ Configure HttpClient for UserService
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// ✅ Ensure DataProtectionKeys directory exists
var keysDirectory = Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionKeys");
if (!Directory.Exists(keysDirectory))
{
    Directory.CreateDirectory(keysDirectory);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();