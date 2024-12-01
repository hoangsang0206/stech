using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using STech.Config;
using STech.Data.Models;
using STech.Services;
using STech.Services.Services;
using Stripe;
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

    //options.Events = new CookieAuthenticationEvents
    //{
    //    OnRedirectToLogin = context =>
    //    {
    //        context.HttpContext.Session.SetInt32("MustShowLoginForm", 1);
    //        context.Response.Redirect(context.RedirectUri);
    //        return Task.CompletedTask;
    //    }
    //};
})
.AddFacebook(facebookOptions =>
{
    IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");

    facebookOptions.AppId = facebookAuthNSection["AppId"] ?? "";
    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"] ?? "";
    facebookOptions.SaveTokens = true;
    facebookOptions.ClaimActions.MapJsonKey("picture", "picture.data.url");
    facebookOptions.Fields.Add("picture");

    facebookOptions.Events = new OAuthEvents
    {
        OnCreatingTicket = context =>
        {
            string picture = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").GetString() ?? "";
            context.Identity?.AddClaim(new Claim("picture", picture));
            return Task.CompletedTask;
        },
        OnAccessDenied = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
    };

})
.AddGoogle(googleOptions =>
{
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

    googleOptions.ClientId = googleAuthNSection["ClientId"] ?? "";
    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"] ?? "";
    googleOptions.SaveTokens = true;
    googleOptions.ClaimActions.MapJsonKey("picture", "picture");

    googleOptions.Events = new OAuthEvents
    {
        OnAccessDenied = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
    };
});


builder.Services.AddDbContext<StechDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("STechDb"));
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, STech.Services.Services.ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerService, STech.Services.Services.CustomerService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IReviewService, STech.Services.Services.ReviewService>();
builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddSingleton(new AddressService(Path.Combine(builder.Environment.ContentRootPath, "DataFiles", "Address")));

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAzureMapsService, AzureMapsService>(sp =>
{
    IConfiguration configuration = builder.Configuration.GetSection("Azure:Maps");
    return new AzureMapsService(sp.GetRequiredService<HttpClient>(), configuration["BaseUri"] ?? "", configuration["SubscriptionKey"] ?? "");
});

builder.Services.AddScoped<IAzureStorageService, AzureStorageService>(sp =>
{
    IConfiguration configuration = builder.Configuration.GetSection("Azure:CloudStorage");
    return new AzureStorageService(configuration["ConnectionString"] ?? "", configuration["BlobContainerName"] ?? "", configuration["BlobUrl"] ?? "");
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Payments:Stripe")["SecretKey"];

CloudflareTurnstile.SiteKey = builder.Configuration.GetSection("Cloudflare:Turnstile")["SiteKey"];
CloudflareTurnstile.SecretKey = builder.Configuration.GetSection("Cloudflare:Turnstile")["SecretKey"];
CloudflareTurnstile.ApiUrl = builder.Configuration.GetSection("Cloudflare:Turnstile")["ApiUrl"];

//builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error/500");
app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Areas", "Admin", "wwwroot")),
    RequestPath = "/admin"
});

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
