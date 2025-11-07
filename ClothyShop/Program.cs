using ClothyShop.Components;
using ClothyShop.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ClothyShop.DAL.Repositories;
using ClothyShop.Business.Services;
using StackExchange.Redis;
using ClothyShop.Business.Models.Hubs;
using ClothyShop.Business.Services.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSignalR(e => {
    e.EnableDetailedErrors = true;
    e.MaximumReceiveMessageSize = 102400000;

}).AddJsonProtocol(options => {
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("ClothyShopAuth")
    .AddCookie("ClothyShopAuth", options =>
    {
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Use None if testing on HTTP localhost
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });


builder.Services.AddAuthorization();

// Register the AppSettings configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

#region MongoDB service registration

// register a client singleton
builder.Services.AddSingleton<IMongoClient>(sp => {
    var opts = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    return new MongoClient(opts.MongoDB.ConnectionString);
});

// register the database itself
builder.Services.AddScoped(sp => {
    var opts = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(opts.MongoDB.DatabaseName);
});

#endregion
#region Redis DI
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(appSettings.Redis.ConnectionString);
});

#endregion

#region DI for repositories

builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<UserRepository>();


#endregion
#region DI for services

builder.Services.AddHttpClient("", client =>
{
    client.BaseAddress = new Uri("https://localhost:7203"); // Change to match your backend/server
});

builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddHostedService<ProductPriceUpdaterService>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); //Add this line to enable interactive server-side rendering

app.MapHub<ProductHUB>("/productHub");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    var repo = new ProductRepository(db);
    await repo.EnsureSeededAsync();
}

app.Run();
