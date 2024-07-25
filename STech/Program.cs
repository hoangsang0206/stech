using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using STech.Data.Models;
using STech.Services;
using STech.Services.Services;
using STech.Utils;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.RelativePath != null && apiDesc.RelativePath.StartsWith("api/");
    });
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = new PathString("/");
    options.AccessDeniedPath = new PathString("/access-denied");
    options.ExpireTimeSpan = TimeSpan.FromDays(90);
})
.AddFacebook(facebookOptions =>
{
    IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");

    facebookOptions.AppId = facebookAuthNSection["AppId"] ?? "";
    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"] ?? "";
    facebookOptions.SaveTokens = true;
    facebookOptions.ClaimActions.MapJsonKey("picture", "picture.data.url");
    facebookOptions.Fields.Add("picture");

    facebookOptions.Events.OnCreatingTicket = context =>
    {
        string picture = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").GetString() ?? "";
        context.Identity?.AddClaim(new Claim("picture", picture));
        return Task.CompletedTask;
    };
})
.AddGoogle(googleOptions =>
{
    IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Google");

    googleOptions.ClientId = facebookAuthNSection["ClientId"] ?? "";
    googleOptions.ClientSecret = facebookAuthNSection["ClientSecret"] ?? "";
    googleOptions.SaveTokens = true;
    googleOptions.ClaimActions.MapJsonKey("picture", "picture");
});


builder.Services.AddDbContext<StechDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("STechDb"));
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

builder.Services.AddSingleton(new AddressService(Path.Combine(builder.Environment.ContentRootPath, "..", "STech.Data")));

builder.Services.AddHttpClient<IGeocodioService, GeocodioService>(client =>
{
    client.BaseAddress = new Uri("https://api.opencagedata.com/geocode/v1/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddSingleton<IGeocodioService, GeocodioService>(sp =>
    new GeocodioService(sp.GetRequiredService<HttpClient>(), builder.Configuration.GetSection("OpenCageGeocodio")["ApiKey"] ?? "")
);

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseStatusCodePages();
}


app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
