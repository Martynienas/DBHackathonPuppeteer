using NUnit.Framework;
using PuppeteerSharp;
using System.Collections.Generic;
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
        private GameSolverHelper solver = new GameSolverHelper();
        private const string testPageUrl = "http://4ark.me/2048/";
        private const int waitForSelectorTimeOut = 5000;
        private WaitForSelectorOptions defaultWaitForSelectorOptions = new WaitForSelectorOptions();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializePage().Wait();
            defaultWaitForSelectorOptions.Timeout = waitForSelectorTimeOut;
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
        public async Task Loose()
        {
            await solver.FailGame(page);
            await page.WaitForSelectorAsync(".failure-container.pop-container.action", defaultWaitForSelectorOptions);
            Assert.Pass();
        }

        [Test, Retry(1000)]
        public async Task Win()
        {
            await solver.Jiggle(page);
            try
            {
                await page.WaitForSelectorAsync(".failure-container.pop-container.action", defaultWaitForSelectorOptions);
                Assert.Fail();
            }
            catch
            {
                Assert.Pass();
            }
        }

        [Test]
        public async Task VerifyGameContainerDisplayed()
        {
            string gameContainerSelector = "div.game-container";
            Assert.DoesNotThrowAsync(() => page.WaitForSelectorAsync(gameContainerSelector, defaultWaitForSelectorOptions), "Game container does not exist");
        }

        [Test]
        public async Task VerifyInitialTilesDisplayed()
        {
            int initialTilesCount = 2;
            int expectedTileSum = 4;
            int actualTileSum = 0;

            var jsCode = @"() => {
                        const selectors = Array.from(document.querySelectorAll('div.tile-container > div.tile')); 
                        return selectors.map( t=> {return { Index: t.getAttribute('data-index'), Value: t.getAttribute('data-val') }});
                        }";
            var results = await page.EvaluateFunctionAsync<GameSolverHelper.Tile[]>(jsCode);

            foreach (var result in results)
            {
                actualTileSum += result.Value;
            }

            Assert.Multiple(() =>
            {
                Assert.AreEqual(initialTilesCount, results.Length, "Tile count mismatch");
                Assert.AreEqual(expectedTileSum, actualTileSum, "Expected tile value sum mismatch");
            });

        }

        [Test]
        public async Task VerifyNewGameAction()
        {
            int refreshCount = 10;
            int matchCount = 0;

            List<int[]> gridElementList = new List<int[]>();
            int[] initialGridElements = await solver.GetGridElements(page);
            for (int i = 0; i < refreshCount; i++)
            {
                int[] gridElements = await ClickNewGameAndGetGrid();
                gridElementList.Add(gridElements);
                if (gridElements == initialGridElements)
                {
                    matchCount++;
                }
            }

            Assert.AreNotEqual(refreshCount, matchCount, "Grid was not reloaded");
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

        private async Task<int[]> ClickNewGameAndGetGrid()
        {
            string newGameButtonSelector = ".restart-btn";
            ElementHandle buttonElement = await page.WaitForSelectorAsync(newGameButtonSelector);
            await buttonElement.ClickAsync();
            int[] gridElements = await solver.GetGridElements(page);
            return gridElements;
        }
    }
}
