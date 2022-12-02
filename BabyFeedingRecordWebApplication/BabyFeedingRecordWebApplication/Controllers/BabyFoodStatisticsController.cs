using BabyFeedingRecordWebApplication.Data;
using BabyFeedingRecordWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace BabyFeedingRecordWebApplication.Controllers
{
    public class BabyFoodStatisticsController : Controller
    {
        private readonly BabyFeedingRecordWebApplicationContext _context;

        public BabyFoodStatisticsController(BabyFeedingRecordWebApplicationContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> RecordOfDay(string? queryDateStr)
        {
            var queryDateStrAry = queryDateStr.Split('/');
            DateTime queryDate = new DateTime(
                int.Parse(queryDateStrAry[0]),
                int.Parse(queryDateStrAry[1]),
                int.Parse(queryDateStrAry[2]),
                0,
                0,
                0
                );
            var feedingRecords = await _context.FeedingRecord.Where(a =>
                a.FeedingTime.Year == queryDate.Year &&
                a.FeedingTime.Month == queryDate.Month &&
                a.FeedingTime.Day == queryDate.Day
                ).Select(a => a).ToListAsync();
            return View(feedingRecords);
        }

        // GET: BabyFoods
        public async Task<IActionResult> Index(int totalDay=7)
        {
            var feedingRecord=await _context.FeedingRecord.ToListAsync();
            var babyFoods = await _context.BabyFood.ToListAsync();
            feedingRecord = feedingRecord.Where(a => a.BabyFoodVolume > 0).Select(a => a).ToList();
            var NowDate=DateTime.Now;
            for(int i=0;i<babyFoods.Count;i++)
            {
                babyFoods[i].WholeDates = feedingRecord.Where(a => a.Memo.Contains(babyFoods[i].Name) && (NowDate- a.FeedingTime).TotalDays<totalDay).Select(a => DateOnly.FromDateTime(a.FeedingTime)).ToList();
                babyFoods[i].ConsecutiveDates=BabyFood.getLongestConsecutiveDates(babyFoods[i].WholeDates);
            }

           
            babyFoods= babyFoods.OrderBy(a =>
            {
                if (a.WholeDates == null)
                    return DateOnly.MinValue;
                else if (a.WholeDates.Count == 0)
                    return DateOnly.MinValue;
                return a.WholeDates.Last();
            }
            ).Reverse().ToList();
            ViewData["BabyFoodDuration"] = totalDay;
            return View(babyFoods);
        }

        // GET: BabyFoods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BabyFood == null)
            {
                return NotFound();
            }

            var babyFood = await _context.BabyFood
                .FirstOrDefaultAsync(m => m.Id == id);
            if (babyFood == null)
            {
                return NotFound();
            }

            var feedingRecord = await _context.FeedingRecord.ToListAsync();
            feedingRecord = feedingRecord.Where(a => a.Memo!=null).Select(a => a).ToList();
            feedingRecord = feedingRecord.Where(a => a.Memo.Contains(babyFood.Name)).Select(a => a).ToList();
            babyFood.WholeDates = feedingRecord.Select(a => DateOnly.FromDateTime(a.FeedingTime)).ToList();
            babyFood.ConsecutiveDates = BabyFood.getLongestConsecutiveDates(babyFood.WholeDates);



            return View(babyFood);
        }
        public async Task<IActionResult> BabyFoodTypeList()
        {
            return View(await _context.BabyFood.ToListAsync());
        }
        public IActionResult Login(string? actn=null,string? ctrl=null)
        {
            return RedirectToAction("Login", "Account",new {ctrl=ctrl,actn=actn });
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetInt32("LoginStatus", 1);
            return Redirect(nameof(BabyFoodTypeList));
        }

        // GET: BabyFoods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BabyFoods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] BabyFood babyFood)
        {
            if (ModelState.IsValid)
            {
                _context.Add(babyFood);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(babyFood);
        }

        // GET: BabyFoods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BabyFood == null)
            {
                return NotFound();
            }

            var babyFood = await _context.BabyFood.FindAsync(id);
            if (babyFood == null)
            {
                return NotFound();
            }
            return View(babyFood);
        }

        // POST: BabyFoods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] BabyFood babyFood)
        {
            if (id != babyFood.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(babyFood);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BabyFoodExists(babyFood.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(babyFood);
        }

        // GET: BabyFoods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BabyFood == null)
            {
                return NotFound();
            }

            var babyFood = await _context.BabyFood
                .FirstOrDefaultAsync(m => m.Id == id);
            if (babyFood == null)
            {
                return NotFound();
            }

            return View(babyFood);
        }

        // POST: BabyFoods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BabyFood == null)
            {
                return Problem("Entity set 'BabyFeedingRecordWebApplicationContext.BabyFood'  is null.");
            }
            var babyFood = await _context.BabyFood.FindAsync(id);
            if (babyFood != null)
            {
                _context.BabyFood.Remove(babyFood);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BabyFoodExists(int id)
        {
            return _context.BabyFood.Any(e => e.Id == id);
        }
    }
}
