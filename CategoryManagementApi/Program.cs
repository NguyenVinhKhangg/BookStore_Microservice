using CategoryManagementApi.Data;
using CategoryManagementApi.DTOs;
using CategoryManagementApi.Models;
using CategoryManagementApi.Profiles;
using CategoryManagementApi.Repositories.Implement;
using CategoryManagementApi.Repositories.Interface;
using CategoryManagementApi.Services.Implement;
using CategoryManagementApi.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System.Text;

namespace CategoryManagementApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddOData(opt =>
                {
                    var odataBuilder = new ODataConventionModelBuilder();
                    odataBuilder.EntitySet<CategoryDTO>("Categories");
                    opt.AddRouteComponents("odata", odataBuilder.GetEdmModel())
                        .Select()
                        .Filter()
                        .OrderBy()
                        .Expand()
                        .Count()
                        .SetMaxTop(100);
                });

            // ✅ FIXED JWT AUTHENTICATION - Use same keys as UserManagement API
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],        // ✅ Changed from JwtSettings:Issuer
                    ValidAudience = builder.Configuration["Jwt:Audience"],    // ✅ Changed from JwtSettings:Audience
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")) // ✅ Changed from JwtSettings:SecretKey
                };

                // ✅ Enhanced logging for JWT events
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogError($"JWT Authentication failed: {context.Exception.Message}");
                        logger?.LogError($"JWT Token: {context.Request.Headers.Authorization}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogInformation($"JWT Token validated for user: {context.Principal?.Identity?.Name}");

                        // ✅ Log user claims for debugging
                        var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                        logger?.LogInformation($"User claims: {string.Join(", ", claims ?? new string[0])}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogWarning($"JWT Challenge: {context.Error}, {context.ErrorDescription}");
                        logger?.LogWarning($"Request path: {context.Request.Path}");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogInformation($"JWT Message received for path: {context.Request.Path}");
                        return Task.CompletedTask;
                    }
                };
            });

            // ✅ ADD AUTHORIZATION
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // ✅ ADD JWT SUPPORT TO SWAGGER
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // Đăng ký DbContext
            builder.Services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(CategoryProfile));

            // Đăng ký Repository và Service cho Category
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ✅ MIDDLEWARE ORDER IS IMPORTANT
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}