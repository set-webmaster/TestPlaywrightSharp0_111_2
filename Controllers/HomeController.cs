using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlaywrightSharp;
using TestPlaywrightSharp111_2.Models;
using TestPlaywrightSharp111_2.OptionsClasses;

namespace TestPlaywrightSharp111_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await Playwright.InstallAsync();
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();
            
            var htmlContent = await System.IO.File.ReadAllTextAsync("wwwroot/testPage.html");
            await page.SetContentAsync(htmlContent, LifecycleEvent.Load);

            var headerTemplate = @"<!DOCTYPE html>
                <html>
                    <head>
                        <title>FooterTemplate</title>
                    </head>
                    <body>
                        <div>Test header template</div>
                    </body>
                </html>";


            var bytesResponse = await page.GetPdfAsync(null, 1, true, headerTemplate, "<div>Test footer template</div>", true, false, "", null,
                null, null, new Margin { Bottom = "1in", Top = "2in" }, false);

            FileResult fileResult = new FileContentResult(bytesResponse, "application/pdf");
            fileResult.FileDownloadName = "test.pdf";
            return fileResult;
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

