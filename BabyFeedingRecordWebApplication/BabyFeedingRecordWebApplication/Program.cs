using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BabyFeedingRecordWebApplication.Data;
using NLog;
using NLog.Web;
using BabyFeedingRecordWebApplication.LineBot;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BabyFeedingRecordWebApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BabyFeedingRecordWebApplicationContext") ?? throw new InvalidOperationException("Connection string 'BabyFeedingRecordWebApplicationContext' not found.")));
int k;
// Add services to the container.
LineBot lineBot = new IsRockLineBot() {
    ChannelAccessToken = builder.Configuration["ChannelAccessToken"],
    ChannelSecret = builder.Configuration["ChannelSecret"],
    ServerWebsite = builder.Configuration["webSite"],
    permittedGids = builder.Configuration.GetSection("AllowedLineGroupID").Get<List<string>>(),
    permittedUids = builder.Configuration.GetSection("AllowedLineUID").Get<List<string>>()

};

builder.Services.AddSingleton<LineBot>(lineBot);
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Host.UseNLog();

//session for simple login usage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/FeedingRecords/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//session for simple login usage
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FeedingRecords}/{action=Index}/{id?}");
    //pattern: "{controller=FeedingRecords}/{action=CreateByLineBot}/{id?}");

app.Run();

NLog.LogManager.Shutdown();