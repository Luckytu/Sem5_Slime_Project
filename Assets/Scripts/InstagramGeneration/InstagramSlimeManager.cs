﻿using System;
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

        [SerializeField] private InstagramSlimeGenerator instagramSlimeGenerator;
        [SerializeField] private SimulationInit simulationInit;
        [SerializeField] private Timeline loadingTimeline;
        
        [SerializeField] private UnityEvent<int> onUpdateQueueSlots;
        [SerializeField] private UnityEvent<List<InstagramSlime>> onUpdateAlive;
        [SerializeField] private UnityEvent<List<InstagramSlime>> onUpdateArchived;
        [SerializeField] private int openQueueSlots = 4;

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
            
            slimeQueue.Enqueue(instagramSlimeGenerator.generate("luwucif", 57, 274, 1, 0.1f));
            slimeQueue.Enqueue(instagramSlimeGenerator.generate("mucha_gue", 38, 49, 0, 0.1f));
            slimeQueue.Enqueue(instagramSlimeGenerator.generate("the_blunicorn", 1164, 1372, 346, 0.1f));
            slimeQueue.Enqueue(instagramSlimeGenerator.generate("laramzp", 854, 659, 32, 0.1f));
            slimeQueue.Enqueue(instagramSlimeGenerator.generate("einfaltslos", 653, 945, 153, 0.1f));
            generate();
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
            onUpdateQueueSlots.Invoke(-1);
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
        
        public void resetSimulation()
        {
            StartCoroutine(startSimulation());
        }

        private IEnumerator startSimulation()
        {
            simulationSettings.currentSpeciesAmount = aliveSlimes.Count;
            simulationInit.useDefaultSpeciesSettings = false;
            simulationInit.speciesSettings = speciesSettings;
            simulationInit.startSimulation();
            yield break;
        }
        
        public void generate()
        {
            //kill ones that exceed max Lifetime
            killByLifetime();
            
            //kill ones that exceed low population
            killByPopulation();
            
            //fill up empty slots with slimes from queue
            while (slimeQueue.Count > 0 && aliveSlimes.Count < simulationSettings.maxSpeciesAmount)
            {
                Debug.Log("queue " + slimeQueue.Count);
                aliveSlimes.Add(slimeQueue.Dequeue());
                openQueueSlots++;
                onUpdateQueueSlots.Invoke(1);
            }
            
            Debug.Log("aliveSlimes " + aliveSlimes.Count);
            
            //redistribute and recalculate populations
            int totalPopulation = 0;
            foreach (InstagramSlime slime in aliveSlimes)
            {
                totalPopulation += slime.species.alivePopulation;
            }

            int previousPopulation = 0;
            for(int i = 0; i < aliveSlimes.Count; i++)
            {
                int population = (int)(((float)aliveSlimes[i].species.alivePopulation / (float)totalPopulation) * simulationSettings.maxEntityAmount);
                Species species = aliveSlimes[i].species;
                
                if (i == aliveSlimes.Count - 1 && previousPopulation + population > simulationSettings.maxEntityAmount)
                {
                    population = simulationSettings.maxEntityAmount - previousPopulation;
                }
                
                species.population = population;
                species.alivePopulation = population;
                species.offset = previousPopulation;
                
                aliveSlimes[i].species = species;
                previousPopulation += population;
            }
            
            //gather species for simulation
            speciesSettings.species = aliveSlimes.Select(s => s.species).ToArray();
            
            onUpdateAlive.Invoke(aliveSlimes);
            
            //call resetSimulation from Timeline
            loadingTimeline.startTimeLine();
        }
        
        private void killByLifetime()
        {
            if (aliveSlimes.Count <= simulationSettings.minSpeciesAmount)
            {
                return;
            }
            
            List<InstagramSlime> orderedByLifetime = aliveSlimes.OrderByDescending(s => s.lifeTime).ToList();

            foreach (InstagramSlime slime in orderedByLifetime)
            {
                if (slime.lifeTime > randomSettings.maximumLifeTime)
                {
                    archivedSlimes.Add(slime);
                    aliveSlimes.Remove(slime);
                    simulationSettings.currentSpeciesAmount = aliveSlimes.Count;
                }
                
                if (aliveSlimes.Count <= simulationSettings.minSpeciesAmount)
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
                }
                
                if (aliveSlimes.Count <= simulationSettings.minSpeciesAmount)
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

            int followers = Int32.Parse(followerMatch);
            int following = Int32.Parse(followingMatch);
            int posts = Int32.Parse(postsMatch);
            
            createSlime(username, posts, followers, following);
        }

        private void createSlime(string username, int posts, int followers, int following)
        {
            InstagramSlime slime = instagramSlimeGenerator.generate(username, followers, following, posts, speciesSettings.spawnMargin);
            
            slimeQueue.Enqueue(slime);
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