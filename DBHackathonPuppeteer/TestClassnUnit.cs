using NUnit.Framework;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBHackathonPuppeteer
{
    [TestFixture]
    class TestClassnUnit
    {
        private Browser browser;
        private Page page;
        private GameSolverHelper solver = new GameSolverHelper();
        private const string testPageUrl = "http://4ark.me/2048/";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializePage().Wait();

            solver = new GameSolverHelper();
        }

        [SetUp]
        public void SetUp()
        {
            //TODO: Clear storage
            page.GoToAsync(testPageUrl).Wait();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            page.Dispose();
        }

        [Test]
        public async Task VerifyUIStrings()
        {
            Assert.AreEqual("2048", await GetText("h1.title"));
            Assert.AreEqual("SCORE", await GetText(".score-container > .title"));
            Assert.AreEqual("BEST", await GetText(".best-container > .title"));
            Assert.AreEqual("New Game", await GetText(".restart-btn"));
            Assert.AreEqual("Join the numbers and get to the 2048 tile!", await GetText(".above-game"));
            Assert.AreEqual("Crafted with by @4Ark/GitHub", await GetText(".footer"));
            //Assert.AreEqual(":( FIALURE", await GetText(".failure-container pop-container"));
            //Assert.AreEqual(":) WINNING", await GetText(".winning-container pop-container"));
        }

        [Test]
        public async Task VerifyLayout()
        {

        }

        [Test]
        public async Task VerifyUiActions()
        {
            int refreshCount = 3;
            int matchCount = 0;

            List<int[]> gridElementList = new List<int[]>();
            int[] initialGridElements = await solver.GetGridElements(page);
            for (int i = 0; i < refreshCount; i++)
            {
                int[] gridElements = await ClickNewGameAndGetGrid();
                gridElementList.Add(gridElements);
                if(gridElements == initialGridElements)
                {
                    matchCount++;
                }
            }

            Assert.AreNotEqual(refreshCount, matchCount, "Grid was not reloaded");
        }

        private async Task<int[]> ClickNewGameAndGetGrid()
        {
            string newGameButtonSelector = ".restart-btn";            
            ElementHandle buttonElement = await page.WaitForSelectorAsync(newGameButtonSelector);
            await buttonElement.ClickAsync();
            int[] gridElements = await solver.GetGridElements(page);
            return gridElements;
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

        private async Task<string> GetText(string selector)
        {
            ElementHandle element = await page.WaitForSelectorAsync(selector);
            return await GetText(element);
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
