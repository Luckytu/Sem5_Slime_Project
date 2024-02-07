using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GameFlow;
using UnityEngine;
using Slime;
using Slime.Settings;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace InstagramGeneration
{
    public class InstagramSlimeManager : MonoBehaviour
    {
        [SerializeField] private RandomGenerationSettings randomSettings;
        [SerializeField] private SimulationSettings simulationSettings;
        [SerializeField] private SpeciesSettings speciesSettings;

        [SerializeField] private SimulationInit simulationInit;
        [SerializeField] private Timeline loadingTimeline;
        
        [SerializeField] private UnityEvent<int> onUpdateQueueSlots;
        [SerializeField] private UnityEvent<List<InstagramSlime>> onUpdateAlive;
        [SerializeField] private UnityEvent<List<InstagramSlime>> onUpdateArchived;
        [SerializeField] private UnityEvent<List<string>> onUpdateQueue;
        [SerializeField] private int openQueueSlots;

        [SerializeField] private float coolDown = 14;
        [SerializeField] private float coolDownVariance = 5;
        
        private Queue<string> profileQueue;
        private Queue<InstagramSlime> slimeQueue;

        private List<InstagramSlime> aliveSlimes;
        private List<InstagramSlime> archivedSlimes;

        private static HttpClient httpClient = new HttpClient();
        private bool canFetchNewProfile = true;
        
        private void Start()
        {
            profileQueue = new Queue<string>();
            slimeQueue = new Queue<InstagramSlime>();

            aliveSlimes = new List<InstagramSlime>();
            archivedSlimes = new List<InstagramSlime>();
            
            //registerProfile("joebiden");
            //registerProfile("taylorswift");
        }

        private void Update()
        {
            foreach (InstagramSlime slime in aliveSlimes)
            {
                slime.lifeTime += Time.deltaTime;
            }
            
            processProfiles();
        }
        
        public void registerProfile(string username)
        {
            profileQueue.Enqueue(username);
            openQueueSlots--;
            onUpdateQueueSlots.Invoke(openQueueSlots);
        }

        private void processProfiles()
        {
            if (!canFetchNewProfile || profileQueue.Count == 0)
            {
                return;
            }

            canFetchNewProfile = false;
            Task.Run(index);
            StartCoroutine(processCooldown());
        }

        private IEnumerator processCooldown()
        {
            Debug.Log("processing");
            
            float randomCooldown = coolDown + Random.Range(-coolDownVariance, coolDownVariance);
            yield return new WaitForSeconds(randomCooldown);
            
            Debug.Log("can process again");
            canFetchNewProfile = true;
        }

        public void generate()
        {
            //kill ones that exceed max Lifetime
            killByLifetime();
            
            //kill ones that exceed low population
            killByPopulation();

            while (simulationSettings.currentSpeciesAmount < simulationSettings.maxSpeciesAmount)
            {
                aliveSlimes.Add(slimeQueue.Dequeue());
                simulationSettings.currentSpeciesAmount = aliveSlimes.Count;
            }
            
            //call resetSimulation from Timeline
            loadingTimeline.startTimeLine();
        }

        public void resetSimulation()
        {
            StartCoroutine(startSimulation());
        }

        private IEnumerator startSimulation()
        {
            simulationInit.startSimulation();
            yield break;
        }
        
        private void killByLifetime()
        {
            if (simulationSettings.currentSpeciesAmount <= simulationSettings.minSpeciesAmount)
            {
                return;
            }
            
            List<InstagramSlime> orderedByLifetime = aliveSlimes.OrderByDescending(o => o.lifeTime).ToList();

            foreach (InstagramSlime slime in orderedByLifetime)
            {
                if (slime.lifeTime > randomSettings.maximumLifeTime)
                {
                    archivedSlimes.Add(slime);
                    aliveSlimes.Remove(slime);
                    simulationSettings.currentSpeciesAmount = aliveSlimes.Count;
                }
                
                if (simulationSettings.currentSpeciesAmount <= simulationSettings.minSpeciesAmount)
                {
                    return;
                }
            }
        }

        private void killByPopulation()
        {
            if (simulationSettings.currentSpeciesAmount <= simulationSettings.minSpeciesAmount)
            {
                return;
            }

            foreach (InstagramSlime slime in aliveSlimes)
            {
                if (slime.species.alivePopulation 
                    >= simulationSettings.maxEntityAmount * randomSettings.populationDeathCutoffRatio 
                    && slime.lifeTime > randomSettings.minimumLifeTime)
                {
                    archivedSlimes.Add(slime);
                    aliveSlimes.Remove(slime);
                    simulationSettings.currentSpeciesAmount = aliveSlimes.Count;
                }
                
                if (simulationSettings.currentSpeciesAmount <= simulationSettings.minSpeciesAmount)
                {
                    return;
                }
            }
        }
            
        private async Task index()
        {
            Debug.Log("started index");
            
            string username = profileQueue.Dequeue();
            string content = "ERROR";
            
            try
            {
                //Use this for debugging
                //content = await getResponse("https://www.scrapethissite.com/").ConfigureAwait(false);
                
                //Use this for instagram
                content = await getResponse("https://www.instagram.com/" + username +"/").ConfigureAwait(false);
                Debug.Log(content);
            }
            catch (Exception e)
            {
                Debug.Log("error in index");
            }
            
            
            string followerRegex = "(?<=meta content=\")(.*?)(?= Followers)";
            string followingRegex = "(?<=Followers, )(.*?)(?= Following)";
            string postsRegex = "(?<=Following, )(.*?)(?= Posts)";
            
            string followerMatch = Regex.Match(content, followerRegex).ToString();
            string followingMatch = Regex.Match(content, followingRegex).ToString();
            string postsMatch = Regex.Match(content, postsRegex).ToString();
            
            followerMatch = followerMatch.Replace(",", string.Empty);
            followingMatch = followingMatch.Replace(",", string.Empty);
            postsMatch = postsMatch.Replace(",", string.Empty);
            
            followerMatch = followerMatch.Replace("K", "000");
            followingMatch = followingMatch.Replace("K", "000");
            postsMatch = postsMatch.Replace("K", "000");
            
            followerMatch = followerMatch.Replace("M", "000000");
            followingMatch = followingMatch.Replace("M", "000000");
            postsMatch = postsMatch.Replace("M", "000000");
            
            Debug.Log(username);
            Debug.Log("followers = " + followerMatch);
            Debug.Log("following = " + followingMatch);
            Debug.Log("posts = " + postsMatch);
            
        }
        
        private async Task<string> getResponse(string url)
        {
            Debug.Log("started response");
            
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
            {
                
                request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
                request.Headers.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                
                
                
                //request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                //request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                //request.Headers.TryAddWithoutValidation("Accept-Language", "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7");
                //request.Headers.TryAddWithoutValidation("Alt-Used", "www.instagram.com");
                //request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                //request.Headers.TryAddWithoutValidation("Host", "www.instagram.com");
                //request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
                //request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                //request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "none");
                //request.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                //request.Headers.TryAddWithoutValidation("User-Agent",
                //    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
                
                
                using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                    using (var streamReader = new StreamReader(decompressedStream))
                    {
                        return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}