using PuppeteerSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DBHackathonPuppeteer
{
    class ScreenshotHelper
    {
        private Page page;

        public ScreenshotHelper(Page page)
        {
            this.page = page;
        }

        public async Task TakeScreenshot(string folderName = "screenshots", string fileNamePrefix = null, string outputFile = null)
        {
            if (outputFile == null)
            {
                outputFile = $"{Directory.GetCurrentDirectory()}/{folderName}/{fileNamePrefix}_{DateTime.Now.ToString("yyyyMMdd_HHmmssffff")}.png";
            }
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            await page.ScreenshotAsync(outputFile);
        }
    }
}
