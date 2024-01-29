using System;
using Global;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Slime.Slime_Settings
{
    [CreateAssetMenu(fileName = "Simulation Settings", menuName = "Settings/Simulation/Simulation Settings", order = 0)]
    public class SimulationSettings : ScriptableObject
    {
        [Header("Graphics Settings")]
        public FilterMode filterMode = FilterMode.Point;
        public GraphicsFormat graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

        [NonSerialized] public RenderTexture foodMap;
        [NonSerialized] public RenderTexture preTrailMap;
        [NonSerialized] public RenderTexture diffusedTrailMap;
        [NonSerialized] public RenderTexture displayMap;
        [NonSerialized] public RenderTexture debugMap;
        
        [NonSerialized] public ComputeBuffer agentBuffer;
        [NonSerialized] public ComputeBuffer speciesBuffer;
        
        public int maxEntityAmount = 1000000;
        public int maxSpeciesAmount = 8;

        public float spawnMargin = 0.1f;
        public float spawnRadius = 0.02f;
        
        public float decayRate;
        public float diffuseRatio;
        public float agentContributionRatio;

        public Color foodColor;
        
        //Randomness of the Simulation
        public float randomVariance;
        public float deathCutoff;

        public void generateTextureMaps()
        {
            GraphicsUtility.createRenderTexture(ref preTrailMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref diffusedTrailMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref displayMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref foodMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
            GraphicsUtility.createRenderTexture(ref debugMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
        }
    }
}