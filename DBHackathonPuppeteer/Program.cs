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
				 Headless = false
			};
			Logger.SetConfiguration();
			Logger.Info("Downloading chromium");
			await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
			using (var browser = await Puppeteer.LaunchAsync(options))
			using (var page = await browser.NewPageAsync())
			{
				Logger.Info("Navigating to puppeteersharp");
				await page.GoToAsync("https://4ark.me/2048/");
				if (!args.Any(arg => arg == "auto-exit"))
				{
					Console.ReadLine();
				}
			}
		}
	}
}