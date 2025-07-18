using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrdersManagementApi.Data;
using OrdersManagementApi.Profiles;
using OrdersManagementApi.Repository;
using OrdersManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<OrderDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<OrderProfile>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();