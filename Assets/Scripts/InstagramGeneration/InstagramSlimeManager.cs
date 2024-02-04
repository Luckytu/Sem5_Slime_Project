using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PuppeteerSharp;
using Slime.Settings;

namespace InstagramGeneration
{
    public class InstagramSlimeManager : MonoBehaviour
    {
        [SerializeField] private RandomGenerationSettings randomSettings;
        [SerializeField] private SimulationSettings simulationSettings;
        
        private Queue<string> generationQueue;
        
        private List<InstagramSlime> aliveSlimes;
        private List<InstagramSlime> archivedSlimes;

        private async void Start()
        {
            await index();
            Debug.Log("heheh");
        }

        private void fetchInstagramProfile()
        {
            
        }
        
        private async Task index()
        {
            Debug.Log("started");
            
            LaunchOptions options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };
            
            IBrowser browser = await Puppeteer.LaunchAsync(options, null);
            Debug.Log("browser");
            
            IPage page = await browser.NewPageAsync();
            Debug.Log("page");

            await page.GoToAsync("https://www.instagram.com/luwucif/");
            Debug.Log("url");
            
            string expression = @"//meta[12]/@content";
            string[] content = await page.EvaluateExpressionAsync<string[]>(expression);
            if (content == null)
            {
                Debug.Log("content is null");
            }

            if (content != null)
                foreach (string str in content)
                {
                    Debug.Log(content);
                }
        }
    }
}