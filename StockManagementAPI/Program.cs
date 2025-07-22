using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using StockManagementApi.Data;
using StockManagementApi.Profiles;
using StockManagementApi.Repositories.Implementations;
using StockManagementApi.Repositories.Interfaces;
using StockManagementApi.Services.Interfaces;
using StockManagementAPI.DTOs;
using StockManagementAPI.Services.Implementations;
using StockManagementAPI.Services.Interfaces;
using System.Text;

namespace StockManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // ✅ SỬA: Cấu hình Authentication với đúng key từ appsettings
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
                    ValidIssuer = builder.Configuration["Jwt:Issuer"], // ✅ SỬA: Jwt không phải JWT
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            // Add OData
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<StockTransactionDTO>("StockTransactions");

            builder.Services.AddControllers()
                .AddOData(options => options
                    .Select()
                    .Filter()
                    .OrderBy()
                    .SetMaxTop(100)
                    .Count()
                    .Expand()
                );

            // Add DbContext
            builder.Services.AddDbContext<StockDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(StockProfile));

            // Add repositories
            builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();

            // Add services
            builder.Services.AddScoped<IStockService, StockService>();

            // Add API Explorer
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Đăng ký RabbitMQ options
            builder.Services.Configure<RabbitMQOptions>(
                builder.Configuration.GetSection("RabbitMQ"));

            // Đăng ký IMessageService
            builder.Services.AddSingleton<IMessageService, RabbitMQService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ✅ THÊM: UseAuthentication() phải được gọi trước UseAuthorization()
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}