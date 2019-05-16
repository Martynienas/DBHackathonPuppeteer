using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBHackathonPuppeteer
{
    class GameSolverHelper
    {
        public static int[] gridTiles = 
           {
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0
           };

        public class Tile
        {
            public int Index { get; set; }
            public int Value { get; set; }
        }

        public async  void GetGridElements(Page page)
        {
            var jsCode = @"() => {
                        const selectors = Array.from(document.querySelectorAll('div.tile-container > div.tile')); 
                        return selectors.map( t=> {return { Index: t.getAttribute('data-index'), Value: t.getAttribute('data-val') }});
                        }";
            var results = await page.EvaluateFunctionAsync<Tile[]>(jsCode);
            foreach (var result in results)
            {
                gridTiles[result.Index] = result.Value;
            }

            //var cleanUrlList = test.Result.ToArray();
        }

    }
}
