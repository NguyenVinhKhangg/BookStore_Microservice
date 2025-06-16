using CategoryManagementApi.Data;
using CategoryManagementApi.Repositories.Interface;
using CategoryManagementApi.Repositories.Implement;
using CategoryManagementApi.Services.Interface;
using CategoryManagementApi.Services.Implement;
using CategoryManagementApi.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using CategoryManagementApi.DTOs;
using CategoryManagementApi.Models;

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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
