using BabyFeedingRecordWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BabyFeedingRecordWebApplication.Data;
using BabyFeedingRecordWebApplication.Models;
using System.Configuration;
using System.Security.Principal;

namespace BabyFeedingRecordWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly BabyFeedingRecordWebApplicationContext _context;
        private readonly ILogger<FeedingRecordsController> _logger;
        private IConfiguration _configuration;
 
        public AccountController(BabyFeedingRecordWebApplicationContext context, ILogger<FeedingRecordsController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger= logger;    
            _configuration= configuration;
        }

        // GET: HomeController
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string? ctrl,string? actn)
        {
            Account account = new Account();

            ViewBag.ReturnUrl = this.Url.Action(actn, ctrl);
            return View(account);
        }

        public bool LoginJudgeMent(Account account)
        {

            var accntDct = _configuration.GetSection("Account").GetChildren().ToDictionary(x => x.Key, y => y.Value);
            if (accntDct.ContainsKey(account.Name))
                if (accntDct[account.Name] == account.Password)
                {
              
                    account.status = string.Empty;
                    return true;
                    //return RedirectToAction("Index", "FeedingRecords", new { startIndex = 1 });
                }

            return false;
        }

        [HttpPost]
        public IActionResult Login(Account account,string? ReturnUrl=null)
        {
            //string? ctrl = ViewData["PrevCtrl"] == null ? null : ViewData["PrevCtrl"].ToString();
            //string? actn = ViewData["PrevActn"] == null ? null : ViewData["PrevActn"].ToString();


            if (LoginJudgeMent(account))
            {
                HttpContext.Session.SetInt32("LoginStatus", 0);
                if (ReturnUrl != null)
                    return Redirect(ReturnUrl);
                return RedirectToAction("Index", "FeedingRecords", new { startIndex = 1 });
            }

            account.status = "Incorrect name or password!";
            return View(account);
        }
    }
}
