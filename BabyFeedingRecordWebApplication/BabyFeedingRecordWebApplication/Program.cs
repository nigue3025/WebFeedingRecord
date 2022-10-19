using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BabyFeedingRecordWebApplication.Data;
using NLog;
using NLog.Web;
using BabyFeedingRecordWebApplication.LineBot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BabyFeedingRecordWebApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BabyFeedingRecordWebApplicationContext") ?? throw new InvalidOperationException("Connection string 'BabyFeedingRecordWebApplicationContext' not found.")));
int k;
// Add services to the container.
LineBot lineBot = new IsRockLineBot() { ChannelAccessToken = builder.Configuration["ChannelAccessToken"], ServerWebsite = builder.Configuration["webSite"] };
builder.Services.AddSingleton<LineBot>(lineBot);
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/FeedingRecords/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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