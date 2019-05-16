using NUnit.Framework;
using PuppeteerSharp;
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
        public async Task VerifyUIStrings()
        {
            var titleSelector = "h1.title";
            ElementHandle title = await page.WaitForSelectorAsync(titleSelector);
            string titlestringstring = await GetText(title);
            Assert.AreEqual("2048", titlestringstring);
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
            await page.GoToAsync(testPageUrl);
        }

        private async Task<string> GetText(ElementHandle element)
        {
            return await GetProperty(element, "innerText");
        }

        private async Task<string> GetProperty(ElementHandle element, string property)
        {
            JSHandle jsHandle = await element.GetPropertyAsync(property);
            string value = await jsHandle.JsonValueAsync<string>();
            return value;
        }
    }
}
