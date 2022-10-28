using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BabyFeedingRecordWebApplication.Data;
using BabyFeedingRecordWebApplication.Models;

namespace BabyFeedingRecordWebApplication.Controllers
{
    public class MovingAvgsController : Controller
    {
        private readonly BabyFeedingRecordWebApplicationContext _context;

        public MovingAvgsController(BabyFeedingRecordWebApplicationContext context)
        {
            _context = context;
        }

        // GET: MovingAvgs
        public async Task<IActionResult> Index(int no=3)
        {
            if (no < 3)
                no = 3;

            var feedingRecords = await _context.FeedingRecord.ToListAsync();
            FeedingStatisticsListBuilder feedingStatisticsListBuilder = new FeedingStatisticsListBuilder();
            feedingStatisticsListBuilder.Add(feedingRecords);
            var statlst = feedingStatisticsListBuilder.generateFeedingStatistics();
            ViewData["MaNum"] = no;
            return View(MovingAvg.getMovingAvg(no,statlst).Where((a,i)=>i<30).Select(a=>a));
        }

        // GET: MovingAvgs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MovingAvg == null)
            {
                return NotFound();
            }

            var movingAvg = await _context.MovingAvg
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movingAvg == null)
            {
                return NotFound();
            }

            return View(movingAvg);
        }

        // GET: MovingAvgs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MovingAvgs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        // GET: MovingAvgs/Edit/5




    

        private bool MovingAvgExists(int id)
        {
          return _context.MovingAvg.Any(e => e.Id == id);
        }
    }
}
