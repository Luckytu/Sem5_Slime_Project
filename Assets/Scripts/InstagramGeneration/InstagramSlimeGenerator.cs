using System.Collections;
using System.Collections.Generic;
using GameFlow.Interpolators;
using InstagramGeneration;
using Slime;
using Slime.Settings;
using UnityEngine;

public class InstagramSlimeGenerator : MonoBehaviour
{
    [SerializeField] private RandomGenerationSettings randomGenerationSettings;
    
    [SerializeField] private Interpolator followerInterpolator;
    [SerializeField] private Interpolator followingInterpolator;
    [SerializeField] private Interpolator postsInterpolator;

    [SerializeField] private int maxFollowers;
    [SerializeField] private int maxFollowing;
    [SerializeField] private int maxPosts;
    
    public InstagramSlime generate(string username, int followers, int following, int posts, float margin)
    {
        Random.InitState(username.GetHashCode());
        
        //Range 0..1, determining the ratio of values as fraction of the max value
        float followersRatio = Mathf.Clamp01((float)followers / (float)maxFollowers);
        float followingRatio = Mathf.Clamp01((float)following / (float)maxFollowing);
        float postsRatio = Mathf.Clamp01((float)posts / (float)maxPosts);

        float moveSpeed = randomGenerationSettings.minMoveSpeed 
                          + followerInterpolator.interpolate(followersRatio) 
                          * (randomGenerationSettings.maxMoveSpeed - randomGenerationSettings.minMoveSpeed);
        
        float turnSpeed = randomGenerationSettings.minTurnSpeed
                          + followingInterpolator.interpolate(followingRatio)
                          * (randomGenerationSettings.maxTurnSpeed - randomGenerationSettings.minTurnSpeed);
        
        float hungerAccumulation = randomGenerationSettings.minHungerAccumulation
                                   + postsInterpolator.interpolate(postsRatio)
                                   * (randomGenerationSettings.maxHungerAccumulation - randomGenerationSettings.minHungerAccumulation);
        
        Species species = new Species()
        {
            color = randomGenerationSettings.color.Evaluate(Random.Range(0f, 1f)),

            spawnPosition = SpeciesSettings.getRandomSpawnPosition(margin),
            population = 125000,
            offset = 0,
            alivePopulation = 125000,
            hungerAccumulation = hungerAccumulation,
            interSpeciesHungerModifier = Random.Range(randomGenerationSettings.minInterSpeciesHungerModifier, randomGenerationSettings.maxInterSpeciesHungerModifier),
            
            moveSpeed = moveSpeed,
            turnSpeed = turnSpeed,
            
            sensorAngle = Random.Range(randomGenerationSettings.minSensorAngle, randomGenerationSettings.maxSensorAngle),
            sensorOffset = Random.Range(randomGenerationSettings.minSensorOffset, randomGenerationSettings.maxSensorOffset)
        };

        return new InstagramSlime(username, followers, following, posts, species);
    }
}
