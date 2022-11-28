using BabyFeedingRecordWebApplication.Data;
using BabyFeedingRecordWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BabyFeedingRecordWebApplication.Controllers
{
    public class FeedingStatisticsController : Controller
    {
        private readonly BabyFeedingRecordWebApplicationContext _context;
        private readonly ILogger<FeedingStatisticsController> _logger;
        public FeedingStatisticsController(BabyFeedingRecordWebApplicationContext context, ILogger<FeedingStatisticsController> logger)
        {
            _context = context;
            _logger = logger;
        }
    
        public async Task<IActionResult> Index(int pageIndex=1)
        {
            var feedingRecords=await _context.FeedingRecord.ToListAsync();
            var babyFoods = await _context.BabyFood.ToListAsync();
            
            FeedingStatisticsListBuilder feedingStatisticsListBuilder=new FeedingStatisticsListBuilder();
            feedingStatisticsListBuilder.Add(feedingRecords);
            ViewData["currPageNo"] = pageIndex;
            var feedingStatisticList = feedingStatisticsListBuilder.generateFeedingStatistics(babyFoods);
           
            return View(feedingStatisticList);
        }


        public async Task<IActionResult> RecordOfDay(string? queryDateStr)
        {
            var queryDateStrAry = queryDateStr.Split('.');
            DateTime queryDate = new DateTime(
                int.Parse(queryDateStrAry[0]),
                int.Parse(queryDateStrAry[1]),
                int.Parse(queryDateStrAry[2]),
                0,
                0,
                0
                );
            var feedingRecords = await _context.FeedingRecord.Where(a=>
                a.FeedingTime.Year==queryDate.Year &&
                a.FeedingTime.Month==queryDate.Month &&
                a.FeedingTime.Day==queryDate.Day
                ).Select(a=>a).ToListAsync();
            return View(feedingRecords);
        }

        
    }
}
