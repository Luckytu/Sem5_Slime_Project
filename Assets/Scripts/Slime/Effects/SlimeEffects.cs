using System;
using Global;
using Slime.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Slime.Effects
{
    public class SlimeEffects : Simulation
    {
        [SerializeField] private ComputeShader effectsShader;
        [SerializeField] private SpeciesSettings speciesSettings;
        [SerializeField] private EffectsSettings effectsSettings;
        [SerializeField] private SpeciesDataDisplay speciesDataDisplay;
        [SerializeField] private EntityDataDisplay entityDataDisplay;
        [SerializeField] private Renderer canvasRenderer;
        [SerializeField] private int entityReadAmount;
        private int entityReadOffset = 0;
 
        [SerializeField] private float pixelSize = 1.0f;

        [SerializeField] private float whiteNoiseAmount = 0.1f;
        
        private bool _scanFoodMap = false;
        public bool scanFoodMap
        {
            get => _scanFoodMap;
            set
            {
                _scanFoodMap = value;
                effectsShader.SetBool("scanFoodMap", scanFoodMap);
            }
        }
        
        private Species[] species;
        private Entity[] entities;
        
        private const int effectsKernel = 0;
        
        [System.Serializable]
        public struct PixelatedArea
        {
            public int bottom;
            public int left;

            public int top;
            public int right;
        }
        [SerializeField] private PixelatedArea[] pixelatedAreas;
        private ComputeBuffer pixelatedAreasBuffer;
        
        private void Start()
        {
            resetPixelization();
            
            effectsShader.SetTexture(effectsKernel, "displayMap", simulationSettings.displayMap);
            effectsShader.SetTexture(effectsKernel, "processedMap", simulationSettings.effectsMap);
            
            effectsShader.SetInt("width", GameSettings.width);
            effectsShader.SetInt("height", GameSettings.height);
            
            effectsShader.SetFloat("pixelSize", pixelSize);
            
            effectsShader.SetFloat("whiteNoiseRatio", whiteNoiseAmount);
        }

        private void Update()
        {
            if (species != null)
            {
                speciesDataDisplay.species = species;
            }

            entityDataDisplay.entities = entities;
            
            readEntities();
        }

        private void FixedUpdate()
        {
            effectsShader.SetFloat("pixelSize", pixelSize);
            effectsShader.SetFloat("whiteNoiseRatio", whiteNoiseAmount);
        }

        public void applyEffects()
        {
            if (species != null)
            {
                if (species.Length != speciesSettings.species.Length)
                {
                    species = new Species[speciesSettings.species.Length];
                }
                
                simulationSettings.speciesBuffer.GetData(species, 0, 0, speciesSettings.species.Length);
                speciesSettings.species = species;
            }
            
            effectsShader.SetFloat("time", Time.fixedTime);
            GraphicsUtility.dispatch(ref effectsShader, effectsKernel, GameSettings.width, GameSettings.height);
        }

        private void readEntities()
        {
            int range = entityReadAmount / speciesSettings.species.Length;

            for (int i = 0; i < speciesSettings.species.Length; i++)
            {
                int remainder = entityReadAmount % speciesSettings.species.Length;
                
                //if on the last loop, add the remainder
                int correctedRange = i == speciesSettings.species.Length - 1 ? range + remainder : range;
                
                int bufferStart = (((simulationSettings.maxEntityAmount / speciesSettings.species.Length) * i) + entityReadOffset) % simulationSettings.maxEntityAmount; 
                simulationSettings.agentBuffer.GetData(entities, i * range, bufferStart, correctedRange);
            }
        }

        public void randomizeEntityReadOffset()
        {
            entityReadOffset = Random.Range(0, 200000);
        }
        
        public void initializeSpecies()
        {
            species = new Species[speciesSettings.species.Length];
            
            speciesDataDisplay.instantiateDataDisplays(speciesSettings.species.Length);
        }

        public void initializeEntities()
        {
            entities = new Entity[entityReadAmount];
            
            entityDataDisplay.instantiateDataDisplays(entityReadAmount);
        }

        private float fraction;
        private int amount = 1;
        public void interpolatePixelization(float t)
        {
            float interval = 1.0f / effectsSettings.maxAreasAmount;
            
            if (t < fraction)
            {
                return;
            }
            
            fraction += interval;
            amount++;
            
            pixelatedAreas = new PixelatedArea[amount];
            
            for (int i = 0; i < amount; i++)
            {
                int minAreaX = effectsSettings.minAreaX + (int)(effectsSettings.growthX * t);
                int minAreaY = effectsSettings.minAreaY + (int)(effectsSettings.growthY * t);
                
                int maxAreaX = effectsSettings.maxAreaX + (int)(effectsSettings.growthX * t);
                int maxAreaY = effectsSettings.maxAreaY + (int)(effectsSettings.growthY * t);
                
                int bottom = Random.Range(0, GameSettings.height - maxAreaY);
                int top = Random.Range(bottom + minAreaY, bottom + maxAreaY);

                int left = Random.Range(0, GameSettings.width - maxAreaX);
                int right = Random.Range(left + minAreaX, left + maxAreaX);
                
                pixelatedAreas[i] = new PixelatedArea()
                {
                    bottom = bottom,
                    left = left,

                    top = top,
                    right = right
                };
            }
            
            GraphicsUtility.Release(pixelatedAreasBuffer);
            effectsShader.SetInt("pixelatedAreasAmount", amount);
            GraphicsUtility.setupShaderBuffer(ref pixelatedAreasBuffer, effectsShader, pixelatedAreas, effectsKernel, "pixelatedAreas");
        }

        public void resetPixelization()
        {
            Debug.Log("reset");
            
            fraction = 0;
            amount = 1;
            
            pixelatedAreas = new PixelatedArea[1];
            GraphicsUtility.Release(pixelatedAreasBuffer);
            effectsShader.SetInt("pixelatedAreasAmount", 0);
            GraphicsUtility.setupShaderBuffer(ref pixelatedAreasBuffer, effectsShader, pixelatedAreas, effectsKernel, "pixelatedAreas");
        }

        public void interpolateWhiteNoise(float t)
        {
            whiteNoiseAmount = t;
        }

        public void interpolatePixelSize(float t)
        {
            pixelSize = Mathf.Max(1, t * effectsSettings.maxPixelSize);
        }

        public void interpolateDistortion(float t)
        {
            float distort = Mathf.Lerp(effectsSettings.minDistort, effectsSettings.maxDistort, t);
            canvasRenderer.material.SetFloat("distortMultiplier", distort);
        }
        
        private void OnDestroy()
        {
            GraphicsUtility.Release(pixelatedAreasBuffer);
        }
    }
}