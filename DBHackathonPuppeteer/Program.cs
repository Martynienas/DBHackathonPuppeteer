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
			Logger.SetConfiguration();
			Logger.Info("Downloading chromium");
			await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
			using (var browser = await Puppeteer.LaunchAsync(options))
			using (var page = await browser.NewPageAsync())
			{
				Logger.Info("Navigating to puppeteersharp");
				await page.GoToAsync("https://www.puppeteersharp.com/examples/index.html");
				Logger.Info("Generating PDF!");
				await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "puppeteersharp.pdf"));
				Logger.Info("Export completed");
				Logger.Info("Navigating to aymen-loukil");
				await page.GoToAsync("https://www.aymen-loukil.com/en/blog-en/google-puppeteer-tutorial-with-examples/");
				Logger.Info("Generating PDF");
				await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "puppeteer-tutorial.pdf"));
				Logger.Info("Export completed");
				if (!args.Any(arg => arg == "auto-exit"))
				{
					Console.ReadLine();
				}
			}
		}
	}
}