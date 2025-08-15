
using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//============= Swagger Customization  ==============
#region Customize Swagger
builder.AddSwaggerSettings();
//builder.Services.AddSwaggerGen(option =>
//{
//    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference= new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id=JwtBearerDefaults.AuthenticationScheme
//                }
//            }, new string[]{}
//        }
//    });
//});
#endregion

//============= Authentication _ Authoriziation ==============
#region Authentication _ Authoriziation Pipline
builder.AddAppAuthentication();

//===============================
//============= V1 ==============
//===============================
//var secret = builder.Configuration.GetValue<string>("ApiSettings:Secret") ?? "";
//var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer");
//var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience");
//// ✅ 1. JWT Bearer Authentication (best for APIs, SPAs, mobile apps)
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer("JwtBearer", options =>
//{
//    //🛠️ Configure JWT token validation here
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        // 👇 Replace with your actual values
//        ValidIssuer = Issuer,
//        ValidAudience = Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
//    };
//});


//===============================
//============= V2 ==============
//===============================
//var settingsSection = builder.Configuration.GetSection("ApiSettings");
//var secret = settingsSection.GetValue<string>("Secret") ?? ""; 
//var issuer = settingsSection.GetValue<string>("Issuer") ?? ""; ;
//var audience = settingsSection.GetValue<string>("Audience") ?? ""; ;
//var key = Encoding.ASCII.GetBytes(secret ?? "");
//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(x =>
//{
//    // 🛠️ Configure JWT token validation here
//    x.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        // 👇 Replace with your actual values
//        ValidIssuer = issuer,
//        ValidAudience = audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret))
//    };
//});


//builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();


//------------ Auto Apply Pending migration -------------
void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}