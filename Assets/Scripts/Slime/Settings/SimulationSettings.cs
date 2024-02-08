using System;
using Global;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Slime.Settings
{
    [CreateAssetMenu(fileName = "Simulation Settings", menuName = "Settings/Simulation/Simulation Settings", order = 0)]
    public class SimulationSettings : ScriptableObject
    {
        [Header("Graphics Settings")]
        public FilterMode filterMode = FilterMode.Point;
        public GraphicsFormat graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

        [NonSerialized] public RenderTexture cameraCaptureMap;
        [NonSerialized] public RenderTexture preFoodMap;
        [NonSerialized] public RenderTexture diffusedFoodMap;

        [NonSerialized] public RenderTexture preTrailMap;
        [NonSerialized] public RenderTexture diffusedTrailMap;
        
        [NonSerialized] public RenderTexture displayMap;
        [NonSerialized] public RenderTexture effectsMap;
        [NonSerialized] public RenderTexture debugMap;
        
        [NonSerialized] public ComputeBuffer agentBuffer;
        [NonSerialized] public ComputeBuffer speciesBuffer;
        
        [Header("Generation Settings")]
        public int maxEntityAmount = 1000000;
        public int maxSpeciesAmount = 8;
        public int minSpeciesAmount = 4;
        
        [Header("Visual Settings")]
        public float spawnRadius = 0.02f;
        public Color foodColor;
        
        [Header("SimulationSettings")]
        public float decayRate;
        public float diffuseRatio;
        public float agentContributionRatio;

        public float minimumPopulationRatio = 0.2f;
        public int maxFoodPheromoneStorage = 1000;

        public int sampleEntitiesAmount = 100;
        public float minSpawnPointDistance = 200;
        public float spawnPointMoveMultiplier = 0.05f;
        
        [Header("Randomness Settings")]
        public float randomVariance;
        public float deathCutoff;

        public void generateTextureMaps()
        {
            GraphicsUtility.createRenderTexture(ref preFoodMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref diffusedFoodMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);

            GraphicsUtility.createRenderTexture(ref preTrailMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref diffusedTrailMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            
            GraphicsUtility.createRenderTexture(ref displayMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref effectsMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref debugMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
        }
        
        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}