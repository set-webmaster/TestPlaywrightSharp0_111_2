using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlaywrightSharp;
using PlaywrightSharp.Chromium;
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

#if false
        /// <summary>
        /// This version uses the new PlaywrightSharp 0.111.2 version
        /// </summary>
        /// <returns></returns>
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
#endif
        /// <summary>
        /// This version uses the new PlaywrightSharp 0.111.2 version
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {

            Console.WriteLine("Downloading chromium");
            var chromium = new ChromiumBrowserType();

            await chromium.CreateBrowserFetcher().DownloadAsync();

            Console.WriteLine("Navigating google");
            await using var browser = await chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var page = await browser.DefaultContext.NewPageAsync();
            var htmlContent = await System.IO.File.ReadAllTextAsync("./wwwroot/testPage.html");
            await page.SetContentAsync(htmlContent, new NavigationOptions()
            {
                WaitUntil = new[] { WaitUntilNavigation.Load }
            });


            var headerTemplate = @"<!DOCTYPE html>
                <html>
                    <head>
                        <title>FooterTemplate</title>
                        <style>body {font-size: 16px; color: lightblue;}</style>
                    </head>
                    <body>
                        <div style=""width: 33%;"">Test header template</div>
                        <div style=""width: 33%;""></div>
                        <div style=""width: 33%; text-align: right;"">Page <span class=""pageNumber""></span></div>
                    </body>
                </html>";

            var options = new PdfOptions()
            {
                Format = PaperFormat.Letter,
                DisplayHeaderFooter = true,
                HeaderTemplate = headerTemplate,
                FooterTemplate = "<div>Test footer template</div>",
                MarginOptions = new MarginOptions { Bottom = "1in", Top = "1in" }
            };

            var stream = await page.GetPdfStreamAsync( options);
            byte[] bytesInStream = new byte[stream.Length];
            stream.Read(bytesInStream, 0, bytesInStream.Length);
            stream.Dispose();

            FileResult fileResult = new FileContentResult(bytesInStream, "application/pdf");
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

