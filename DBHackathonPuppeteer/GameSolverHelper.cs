using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHackathonPuppeteer
{
    class GameSolverHelper
    {
        public class Tile
        {
            public int Index { get; set; }
            public int Value { get; set; }
        }

        public async Task<int[]> GetGridElements(Page page)
        {
            int[] gridTiles =
           {
                0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0
           }; ;

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
                    }
                }
            }
        }

    }
}
