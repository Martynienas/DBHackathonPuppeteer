using NUnit.Framework;
using PuppeteerSharp;
using System.Collections.Generic;
using PuppeteerSharp.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace DBHackathonPuppeteer
{
    [TestFixture]
    class TestClassnUnit
    {
        private Page page;
        private GameSolverHelper solver = new GameSolverHelper();
        private const string testPageUrl = "https://broken-2048.herokuapp.com";
        private const int waitForSelectorTimeOut = 5000;
        private WaitForSelectorOptions defaultWaitForSelectorOptions = new WaitForSelectorOptions();
        private ScreenshotHelper screenshotHelper;
        private string runId;

        public class Score
        {
            public string CurrentScore { get; set; }
            public string BestScore { get; set; }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            runId = $"TestRun_{DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff")}";
            InitializePage().Wait();
            defaultWaitForSelectorOptions.Timeout = waitForSelectorTimeOut;
            solver = new GameSolverHelper();
            screenshotHelper = new ScreenshotHelper(page);
            page.GoToAsync(testPageUrl).Wait();
            screenshotHelper.TakeScreenshot(runId, "OneTimeSetUp").Wait();
        }

        [SetUp]
        public void SetUp()
        {
            ClearLocalStorage().Wait();
            page.GoToAsync(testPageUrl).Wait();
        }

        [TearDown]
        public void TearDown()
        {
            screenshotHelper.TakeScreenshot(runId, "TearDown").Wait();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            screenshotHelper.TakeScreenshot(runId, "OneTimeTearDown").Wait();
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
            Assert.AreEqual(":(\n\nFIALURE", await GetText(".failure-container.pop-container"));
            Assert.AreEqual(":)\n\nWINNING", await GetText(".winning-container.pop-container"));
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
        [Test]
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
                if (gridElements.SequenceEqual(initialGridElements))
                {
                    matchCount++;
                }
            }

            //TODO: Group and check different group count
            Assert.AreNotEqual(refreshCount, matchCount, "Grid was not reloaded");
        }

        [TestCase("div.game-container")]
        [TestCase("div.score-container")]
        [TestCase("div.best-container")]
        public async Task VerifyPageElements(string selector)
        {
            Assert.DoesNotThrowAsync(() => page.WaitForSelectorAsync(selector, defaultWaitForSelectorOptions), $"{selector} element does not exist");
        }

        [Test]
        public async Task VerifyScoring()
        {
            Score initialScore = await GetScore();

            await solver.JiggleUntilFail(page);
            Score scoreAfterGame = await GetScore();

            await ClickNewGame();
            Score scoreAfterNewGame = await GetScore();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("0", initialScore.CurrentScore, "Initial current score mismatch");
                Assert.AreEqual("0", initialScore.BestScore, "Initial best score mismatch");
                Assert.True(IsInteger(initialScore.CurrentScore), "Initial current is not integer");
                Assert.True(IsInteger(initialScore.BestScore), "Initial best score is not integer");
                Assert.AreNotEqual("0", scoreAfterGame.CurrentScore, "Current score after game mismatch");
                Assert.AreNotEqual("0", scoreAfterGame.BestScore, "Best score after game mismatch");
                Assert.True(IsInteger(scoreAfterGame.CurrentScore), "Current score after game is not integer");
                Assert.True(IsInteger(scoreAfterGame.BestScore), "Best score after game is not integer");
                Assert.AreEqual("0", scoreAfterNewGame.CurrentScore, "Current score for new game mismatch");
                Assert.AreEqual(scoreAfterGame.BestScore, scoreAfterNewGame.BestScore, "Best score for new game mismatch");
                Assert.True(IsInteger(scoreAfterGame.CurrentScore), "Current score for new game is not integer");
                Assert.True(IsInteger(scoreAfterGame.BestScore), "Best score for new game is not integer");
            });
        }

        [Test]
        public async Task VerifyScoreCalculation()
        {
            int[] initialGrid = await solver.GetGridElements(page);
            int initialSum = initialGrid.Sum();
            await page.Keyboard.PressAsync(Key.ArrowDown);
            await page.Keyboard.PressAsync(Key.ArrowRight);
            int[] newGrid = await solver.GetGridElements(page);
            int newSum = newGrid.Sum();

            Score score = await GetScore();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("4", score.CurrentScore, "Initial current score mismatch");
                Assert.AreEqual("4", score.BestScore, "Initial best score mismatch");
                Assert.True(newGrid.Contains(4), "Tile #4 were not displayed");
            });
        }

        [Test]
        public async Task VerifyAdvancedScoreCalculation()
        {
            int moveCount = 1000;
            int error = 0;

            for (int i = 0; i < moveCount; i++)
            {
                int[] initialGrid = await solver.GetGridElements(page);
                int initialTileCount = GetTileCount(initialGrid);
                Score initialScore = await GetScore();
                await MakeRandomMove();
                int[] gridAfterMove = await solver.GetGridElements(page);
                int tileCountAfterMove = GetTileCount(gridAfterMove);
                Score scoreAfterMove = await GetScore();

                if ((tileCountAfterMove == initialTileCount) && (initialScore.CurrentScore == scoreAfterMove.CurrentScore))
                {
                    error++;

                    if(error <= 100)
                    {
                        continue;
                    }

                    try
                    {
                        await page.WaitForSelectorAsync(".failure-container.pop-container.action", defaultWaitForSelectorOptions);
                    }
                    catch (Exception)
                    {
                        Assert.Fail("Score calculation error");
                    }
                }
            }
        }

        private async Task MakeRandomMove()
        {
            Random rnd = new Random();
            int random = rnd.Next(1, 5);

            switch (random)
            {
                case 1:
                    await page.Keyboard.PressAsync(Key.ArrowDown);
                    break;
                case 2:
                    await page.Keyboard.PressAsync(Key.ArrowUp);
                    break;
                case 3:
                    await page.Keyboard.PressAsync(Key.ArrowLeft);
                    break;
                case 4:
                    await page.Keyboard.PressAsync(Key.ArrowRight);
                    break;
            }
        }
        
        private int GetTileCount(int[] grid)
        {
            int tileCount = 0;

            foreach(int i in grid)
            {
                if (i == 0) tileCount++;
            }

            return tileCount;
        }

        private bool IsInteger(string score)
        {
            return int.TryParse(score, out int result);
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
            await ClickNewGame();
            int[] gridElements = await solver.GetGridElements(page);
            return gridElements;
        }

        private async Task ClickNewGame()
        {
            string newGameButtonSelector = ".restart-btn";
            ElementHandle buttonElement = await page.WaitForSelectorAsync(newGameButtonSelector);
            await buttonElement.ClickAsync();
        }

        private async Task ClearLocalStorage()
        {
            string clearStorage = "javascript:localStorage.clear();";
            await page.EvaluateExpressionAsync(clearStorage);
        }

        private async Task<Score> GetScore()
        {
            string currentScore = await GetText("div.score-container > p.score");
            string bestScore = await GetText("div.best-container > p.score");

            Score score = new Score
            {
                CurrentScore = currentScore,
                BestScore = bestScore
            };

            return score;
        }
    }
}