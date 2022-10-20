using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BabyFeedingRecordWebApplication.Data;
using BabyFeedingRecordWebApplication.Models;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Concurrent;
using isRock.LineBot;
using System.Configuration;

namespace BabyFeedingRecordWebApplication.Controllers
{
    public class FeedingRecordsController : Controller
    {
      

        private readonly BabyFeedingRecordWebApplicationContext _context;
        private readonly ILogger<FeedingRecordsController> _logger;
        private IConfiguration Configuration;
        private readonly LineBot.LineBot lineBot;
        public FeedingRecordsController(BabyFeedingRecordWebApplicationContext context, ILogger<FeedingRecordsController> logger, IConfiguration configuration,LineBot.LineBot theLineBot)
        {
            _logger = logger;
            _context = context;
            Configuration = configuration;
            lineBot = theLineBot;
        }

        // GET: FeedingRecords
        public async Task<IActionResult> Index(int? startIndex=1)
        {
            //依照feedingTime時間倒序排列
            ViewData["currPageNo"] = startIndex;
            startIndex = (startIndex-1) * 30;
            var feedRecords=await _context.FeedingRecord.ToListAsync();
            ViewData["totalPageNo"] =(int) Math.Ceiling(feedRecords.Count() / 30.0);
            feedRecords.Sort((a, b) => a.FeedingTime.CompareTo(b.FeedingTime));
            feedRecords.Reverse();
            //依照給定頁數篩選30筆資料
            feedRecords= feedRecords.Where((o, i) => ((i < (startIndex+30)&& i>=startIndex) )).Select(a => a).ToList();
            
            return View(feedRecords);
         
        }

        // GET: FeedingRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FeedingRecord == null)
            {
                return NotFound();
            }

            var feedingRecord = await _context.FeedingRecord
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedingRecord == null)
            {
                return NotFound();
            }
            
            return View(feedingRecord);
        }

        // GET: FeedingRecords/Create
        public IActionResult Create()
        {
            return View();
        }

        async Task<bool> UpdateFeedingData(string msgStr)
        {
            string outMSg;
            FeedingRecord feedingRecord;
            if (FeedingRecord.CreateFeedingRecord(msgStr, out feedingRecord, out outMSg))
            {
                await Create(feedingRecord);
                return true;
            }
            return false;
           
        }


        [HttpPost]
        public async Task<IActionResult> CreateByLineBot()
        {
            try
            {
                string jsonStr =String.Empty;
                using (var reader = new StreamReader(HttpContext.Request.Body))
                {
                     jsonStr = await reader.ReadToEndAsync();
                }
                _logger.Log(LogLevel.Information, jsonStr);
                
                lineBot.decodeReceivedMessage(jsonStr);
                if(lineBot.LineEvents!=null)
                    foreach(var lineEvent in lineBot.LineEvents)
                    {
                        if (lineEvent == null) continue;
                        if (lineEvent.ReceivedMessage == null) continue;
                        if (await UpdateFeedingData(lineEvent.ReceivedMessage))
                            lineBot.sendMessage(lineEvent.replyToken, $"更新成功,可至 {lineBot.ServerWebsite} 確認或修改, 辛苦了");
                        else
                            lineBot.sendMessage(lineEvent.replyToken, $"更新失敗,請檢查輸入格式,或至 {lineBot.ServerWebsite} 進行更新, 辛苦了");
                    }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Ok();
        }

        // POST: FeedingRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FeedingDate,FeedingTime,MotherMilkVolume,FormularMilkVolume,Memo")] FeedingRecord feedingRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feedingRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedingRecord);
        }

        // GET: FeedingRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FeedingRecord == null)
            {
                return NotFound();
            }

            var feedingRecord = await _context.FeedingRecord.FindAsync(id);
            if (feedingRecord == null)
            {
                return NotFound();
            }
            return View(feedingRecord);
        }

        // POST: FeedingRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FeedingDate,FeedingTime,MotherMilkVolume,FormularMilkVolume,Memo")] FeedingRecord feedingRecord)
        {
            if (id != feedingRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedingRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedingRecordExists(feedingRecord.Id))
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
            return View(feedingRecord);
        }

        // GET: FeedingRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FeedingRecord == null)
            {
                return NotFound();
            }

            var feedingRecord = await _context.FeedingRecord
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedingRecord == null)
            {
                return NotFound();
            }

            return View(feedingRecord);
        }

        // POST: FeedingRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FeedingRecord == null)
            {
                return Problem("Entity set 'BabyFeedingRecordWebApplicationContext.FeedingRecord'  is null.");
            }
            var feedingRecord = await _context.FeedingRecord.FindAsync(id);
            if (feedingRecord != null)
            {
                _context.FeedingRecord.Remove(feedingRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedingRecordExists(int id)
        {
          return _context.FeedingRecord.Any(e => e.Id == id);
        }
    }
}
