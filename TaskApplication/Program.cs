using AutoMapper;
using Database.DbContexts;
using Domain.Repositories;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Repository.BusinessModels;
using Repository.Mapping_Profile;
using Repository.Repository;
using Service.ServiceModels;
using Service.Services;
using TaskApplication.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));


builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var mapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile<MappingProfile>();
    config.CreateMap<EmployeeViewModel, EmployeeBusiness>();
    config.CreateMap<EmployeeBusiness, EmployeeViewModel>();
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
