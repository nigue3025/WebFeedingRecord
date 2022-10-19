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
    
        public async Task<IActionResult> Index()
        {
            var feedingRecords=await _context.FeedingRecord.ToListAsync();
            
            FeedingStatisticsListBuilder feedingStatisticsListBuilder=new FeedingStatisticsListBuilder();
            feedingStatisticsListBuilder.Add(feedingRecords);
         
            return View(feedingStatisticsListBuilder.generateFeedingStatistics());
        }
    }
}
