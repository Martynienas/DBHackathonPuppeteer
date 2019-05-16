﻿using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHackathonPuppeteer
{
    class GameSolverHelper
    {
        public static int[] emptyGrid()
        {
            return new int[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            };
        }

        public class Tile
        {
            public int Index { get; set; }
            public int Value { get; set; }
        }

        public async Task<int[]> GetGridElements(Page page)
        {
            int[] gridTiles = emptyGrid();
            var jsCode = @"() => {
                        const selectors = Array.from(document.querySelectorAll('div.tile-container > div.tile')); 
                        return selectors.map( t=> {return { Index: t.getAttribute('data-index'), Value: t.getAttribute('data-val') }});
                        }";
            var results = await page.EvaluateFunctionAsync<Tile[]>(jsCode);
            foreach (var result in results)
            {
                gridTiles[result.Index] = result.Value;
            }

            return gridTiles;

        }

        public async Task Jiggle(Page page)
        {
            
            for (int i = 1; i < 10000; i++)
            {
                int[] tempGrid = await GetGridElements(page);
                await page.Keyboard.PressAsync("ArrowUp");
                await page.Keyboard.PressAsync("ArrowRight");
                int[] tempGrid2 = await GetGridElements(page);
                bool isEqual = Enumerable.SequenceEqual(tempGrid, tempGrid2);
                if (isEqual)
                {
                    await page.Keyboard.PressAsync("ArrowDown");
                                        if (Enumerable.SequenceEqual(tempGrid, await GetGridElements(page)))
                    {
                        await page.Keyboard.PressAsync("ArrowLeft");
                        if (Enumerable.SequenceEqual(tempGrid, await GetGridElements(page)))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public async Task FailGame(Page page)
        {

            for (int i = 1; i < 10000; i++)
            {
                int[] beforeGrid = await GetGridElements(page);
                await page.Keyboard.PressAsync(Key.ArrowDown);
                await page.Keyboard.PressAsync(Key.ArrowLeft);
                await page.Keyboard.PressAsync(Key.ArrowUp);
                await page.Keyboard.PressAsync(Key.ArrowRight);
                int[] afterGrid = await GetGridElements(page);
                if (beforeGrid.SequenceEqual(afterGrid))
                {
                    break;
                }
            }
        }

        public async Task JiggleUntilFail(Page page)
        {
            int failedActions = 0;
            for (int i = 1; i < 10000; i++)
            {
                int[] tempGrid = await GetGridElements(page);
                await page.Keyboard.PressAsync("ArrowUp");
                await page.Keyboard.PressAsync("ArrowRight");
                int[] tempGrid2 = await GetGridElements(page);
                bool isEqual = Enumerable.SequenceEqual(tempGrid, tempGrid2);
                if (isEqual)
                {
                    if(failedActions >= 3)
                    {
                        break;
                    }

                    await page.Keyboard.PressAsync("ArrowDown");
                    if (Enumerable.SequenceEqual(tempGrid, await GetGridElements(page)))
                    {
                        await page.Keyboard.PressAsync("ArrowLeft");
                        if (Enumerable.SequenceEqual(tempGrid, await GetGridElements(page)))
                        {
                            failedActions++;
                        }
                    }
                }
            }
        }

        public async Task Jigglev2(Page page)
        {
            int failedActions = 0;
            for (int i = 1; i < 10000; i++)
            {
                int[] tempGrid = await GetGridElements(page);
                if (tempGrid[3] == 0)
                {
                    await page.Keyboard.PressAsync("ArrowUp");
                    await page.Keyboard.PressAsync("ArrowRight");
                }
                if (tempGrid[2] == tempGrid[3])
                {
                    await page.Keyboard.PressAsync("ArrowRight");
                    //continue;
                }
                if (tempGrid[7] == tempGrid[3])
                {
                    await page.Keyboard.PressAsync("ArrowUp");
                    continue;
                }

                if ((tempGrid[2] != tempGrid[3]) && (tempGrid[7] != tempGrid[3]))
                {
                    if (tempGrid[1] == tempGrid[2])
                    {
                        await page.Keyboard.PressAsync("ArrowRight");
                        //continue;
                    }
                    if (tempGrid[6] == tempGrid[2])
                    {
                        await page.Keyboard.PressAsync("ArrowUp");
                        //continue;
                    }
                    if (tempGrid[6] == tempGrid[7])
                    {
                        await page.Keyboard.PressAsync("ArrowRight");
                        //continue;
                    }
                    if (tempGrid[11] == tempGrid[7])
                    {
                        await page.Keyboard.PressAsync("ArrowUp");
                        continue;
                    }
                    // ----------------- third world ----------------------
                    if (tempGrid[1] == tempGrid[1])
                    {
                        await page.Keyboard.PressAsync("ArrowRight");
                        //continue;
                    }
                    if (tempGrid[6] == tempGrid[1])
                    {
                        await page.Keyboard.PressAsync("ArrowUp");
                        //continue;
                    }
                    if (tempGrid[6] == tempGrid[6])
                    {
                        await page.Keyboard.PressAsync("ArrowRight");
                        //continue;
                    }
                    if (tempGrid[11] == tempGrid[6])
                    {
                        await page.Keyboard.PressAsync("ArrowUp");
                        //continue;
                    }
                    if (tempGrid[6] == tempGrid[11])
                    {
                        await page.Keyboard.PressAsync("ArrowRight");
                        //continue;
                    }
                    if (tempGrid[11] == tempGrid[11])
                    {
                        await page.Keyboard.PressAsync("ArrowUp");
                        //continue;
                    }
                }

                {
                    int[] tempGrid3 = await GetGridElements(page);
                    await page.Keyboard.PressAsync("ArrowRight");
                    await page.Keyboard.PressAsync("ArrowUp");
                    int[] tempGrid4 = await GetGridElements(page);
                    if (Enumerable.SequenceEqual(tempGrid3, tempGrid4))
                    {
                        await page.Keyboard.PressAsync("ArrowDown");
                        //await page.Keyboard.PressAsync("ArrowUp");
                    }
                }
                int[] tempGrid2 = await GetGridElements(page);
                bool isEqual = Enumerable.SequenceEqual(tempGrid, tempGrid2);
                if (isEqual)
                {
                    {
                        if (failedActions >= 3)
                        {
                            break;
                        }

                        await page.Keyboard.PressAsync("ArrowDown");
                        if (Enumerable.SequenceEqual(tempGrid, await GetGridElements(page)))
                        {
                            await page.Keyboard.PressAsync("ArrowLeft");
                            //if (Enumerable.SequenceEqual(tempGrid, await GetGridElements(page)))
                            //{
                            //    failedActions++;
                            //}
                        }
                    }
                }
            }
        }
    }
}
