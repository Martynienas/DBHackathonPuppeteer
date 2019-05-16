using NUnit.Framework;
using PuppeteerSharp;
using System;
using System.Threading.Tasks;

namespace DBHackathonPuppeteer
{
    [TestFixture]
    class TestClassnUnit
    {
        private Browser browser;
        private Page page;
        private const string testPageUrl = "http://4ark.me/2048/";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializePage().Wait();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            page.Dispose();
            browser.Dispose();
        }

        [Test]
        public async Task AssertNavigationToUrl()
        {
            await page.GoToAsync(testPageUrl);
            Assert.AreEqual(testPageUrl, page.Url);
        }

        private async Task InitializePage()
        {
            string[] browserArgs = { "--start-maximized" };

            var options = new LaunchOptions
            {
                Headless = false,
                Args = browserArgs
            };

            var viewPortOptions = new ViewPortOptions
            {
                Width = 1920,
                Height = 1080
            };

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(options);
            page = await browser.NewPageAsync();
            await page.SetViewportAsync(viewPortOptions);
        }
    }
}
