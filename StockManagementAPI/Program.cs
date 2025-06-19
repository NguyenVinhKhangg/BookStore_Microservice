using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using StockManagementApi.Data;
using StockManagementAPI.DTOs;
using StockManagementApi.Profiles;
using StockManagementApi.Repositories.Implementations;
using StockManagementApi.Repositories.Interfaces;
using StockManagementApi.Services.Interfaces;
using StockManagementAPI.Services.Implementations;
using StockManagementAPI.Services.Interfaces;

namespace StockManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
