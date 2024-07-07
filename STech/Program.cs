using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services;
using STech.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = new PathString("/");
    options.AccessDeniedPath = new PathString("/access-denied");
});

builder.Services.AddDbContext<StechDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("STechDb"));
}, ServiceLifetime.Singleton);

builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IBrandService, BrandService>();
builder.Services.AddSingleton<ISliderService, SliderService>();
builder.Services.AddSingleton<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
