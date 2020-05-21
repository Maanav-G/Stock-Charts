using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YahooFinance.Models;
using YahooFinanceApi;

namespace YahooFinance.Controllers
{
    [Produces ("application/json")]
    public class HomeController : Controller
    {
        [Route("~/api/ApiStockData/{symbol}/{start}/{end}/{period}")]
        [HttpGet]
        public async Task<List<Stock>> GetData(string symbol, string start, string end, string period)
        {
            var p = Period.Daily;
            if (period.ToLower() == "weekly") p = Period.Weekly;
            else if (period.ToLower() == "monthly") p = Period.Monthly;

            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            var history = await Yahoo.GetHistoricalAsync(symbol, startDate, endDate, p);

            List<Stock> models = new List<Stock>();
            foreach (var rec in history)
            {
                models.Add(new Stock
                {
                    Symbol = symbol,
                    Date = rec.DateTime.ToString("yyyy-MM-dd"),
                    Open = rec.Open,
                    High = rec.High,
                    Low = rec.Low,
                    Close = rec.Close,
                    AdjustedClose = rec.AdjustedClose,
                    Volume = rec.Volume
                });
            }
            return models;
        }


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
