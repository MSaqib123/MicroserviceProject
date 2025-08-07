using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Services;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();

SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICouponService, CouponService>();

#region Authentication Configuration
// 🛡️ Authentication Configuration for MVC Web Application
// This section tells ASP.NET Core to use Cookie-based Authentication.
// Cookie authentication is best for:
// ✅ Traditional MVC web applications (with views, forms, sessions)
// ❌ Not ideal for APIs or mobile apps (use JWT instead in those cases)
// ✅ 1. Cookie Authentication (best for MVC web apps
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // ⏳ How long the login session should stay valid
        // In this case, the user will remain logged in for 10 hours unless they log out
        options.ExpireTimeSpan = TimeSpan.FromHours(10);

        // 🔐 This is where unauthenticated users will be redirected to when they try to access a protected page
        // Example: If a user tries to open /Dashboard but is not logged in, they'll be redirected to /Auth/Login
        options.LoginPath = "/Auth/Login";

        // 🚫 This is where users will be sent if they are authenticated but not authorized (don't have permission)
        // Example: Logged in but trying to access an admin-only page
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// 📝 Summary:
// ✔️ This sets up a secure, cookie-based login session for your ASP.NET Core MVC web app.
// ✔️ It is the most common and recommended method for apps using Razor views (not APIs).
// ✔️ Cookies will store a secure identity and ASP.NET will automatically read and validate it on every request.

/*
// 🛡️ AUTHENTICATION CONFIGURATION FOR ASP.NET CORE APPLICATIONS
// This section shows different types of authentication options with comments.

// ✅ 2. JWT Bearer Authentication (best for APIs, SPAs, mobile apps)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    // 🛠️ Configure JWT token validation here
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // 👇 Replace with your actual values
        ValidIssuer = "https://yourdomain.com",
        ValidAudience = "https://yourdomain.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyHere"))
    };
});

// ✅ 3. Google Authentication (best for apps needing Google login)
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        // 👇 You need to set these up in Google Cloud Console
        googleOptions.ClientId = "YourGoogleClientId";
        googleOptions.ClientSecret = "YourGoogleClientSecret";
    });

// ✅ 4. Microsoft Authentication (for Office 365 / Azure AD login)
builder.Services.AddAuthentication()
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = "YourMicrosoftClientId";
        microsoftOptions.ClientSecret = "YourMicrosoftClientSecret";
    });

// ✅ 5. Facebook Authentication (for Facebook login)
builder.Services.AddAuthentication()
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = "YourFacebookAppId";
        facebookOptions.AppSecret = "YourFacebookAppSecret";
    });

// ✅ 6. Windows Authentication (best for intranet apps - enabled in IIS or Kestrel)
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate(); // For Windows domain-based login (SSO experience)

*/
#endregion


var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
