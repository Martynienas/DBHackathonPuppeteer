﻿using NUnit.Framework;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System.Linq;
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
        public async Task ArrowDown()
        {
            var gameSolver = new GameSolverHelper();
            int[] initialGrid = await gameSolver.GetGridElements(page);
            int initialSum = initialGrid.Sum();
            await page.Keyboard.PressAsync(Key.ArrowDown);
            int[] newGrid = await gameSolver.GetGridElements(page);
            int newSum = newGrid.Sum();

            Assert.AreEqual(initialSum + 2, newSum);
            Assert.GreaterOrEqual(newGrid.TakeLast(4).Sum(), initialSum);

            await page.Keyboard.PressAsync(Key.ArrowDown);
        }

        [Test]
        public async Task ArrowUp()
        {
            var gameSolver = new GameSolverHelper();
            int[] initialGrid = await gameSolver.GetGridElements(page);
            int initialSum = initialGrid.Sum();
            await page.Keyboard.PressAsync(Key.ArrowUp);
            int[] newGrid = await gameSolver.GetGridElements(page);
            int newSum = newGrid.Sum();

            Assert.AreEqual(initialSum + 2, newSum);
            Assert.GreaterOrEqual(newGrid.Take(4).Sum(), initialSum);
        }

        [Test]
        public async Task ArrowLeft()
        {
            var gameSolver = new GameSolverHelper();
            int[] initialGrid = await gameSolver.GetGridElements(page);
            int initialSum = initialGrid.Sum();
            await page.Keyboard.PressAsync(Key.ArrowLeft);
            int[] newGrid = await gameSolver.GetGridElements(page);
            int newSum = newGrid.Sum();

            Assert.AreEqual(initialSum + 2, newSum);
            //TODO asser left line
        }

        [Test]
        public async Task ArrowRight()
        {
            var gameSolver = new GameSolverHelper();
            int[] initialGrid = await gameSolver.GetGridElements(page);
            int initialSum = initialGrid.Sum();
            await page.Keyboard.PressAsync(Key.ArrowRight);
            int[] newGrid = await gameSolver.GetGridElements(page);
            int newSum = newGrid.Sum();

            Assert.AreEqual(initialSum + 2, newSum);
            //TODO asser right line
        }


        [Test]
        public async Task VerifyLayout()
        {

        }

        [Test]
        public async Task VerifyUiActions()
        {
            string newGameButtonSelector = ".restart-btn";

            ElementHandle buttonElement = await page.WaitForSelectorAsync(newGameButtonSelector);
            await buttonElement.ClickAsync();
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
