using BookImgManagementApi.Data;
using BookImgManagementApi.DTOs;
using BookImgManagementApi.Repositories;
using BookImgManagementApi.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<BooksImgReadDto>("BooksImgs");

builder.Services.AddControllers()
    .AddOData(opt => opt
        .AddRouteComponents("odata", odataBuilder.GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .SetMaxTop(100)
        .Count()
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BookImgDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IBooksImgRepository, BooksImgRepository>();
builder.Services.AddScoped<IBooksImgService, BooksImgService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
