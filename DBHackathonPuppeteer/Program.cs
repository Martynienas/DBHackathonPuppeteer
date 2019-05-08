using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerSharpPdfDemo
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = true
            };
            Console.WriteLine("Downloading chromium");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            MyLogger.SetConfiguration();
            MyLogger.Info("Navigating to puppeteersharp");
            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://www.puppeteersharp.com/examples/index.html");
                MyLogger.Info("Generating PDF!");
                await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "puppeteersharp.pdf"));
                MyLogger.Info("Export completed");
                await page.GoToAsync("https://www.aymen-loukil.com/en/blog-en/google-puppeteer-tutorial-with-examples/");
                MyLogger.Info("Generating PDF");
                await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "puppeteer-tutorial.pdf"));
                MyLogger.Info("Export completed");
                if (!args.Any(arg => arg == "auto-exit"))
                {
                    Console.ReadLine();
                }
            }
        }
    }
}