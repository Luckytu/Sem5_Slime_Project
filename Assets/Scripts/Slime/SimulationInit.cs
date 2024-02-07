using System;
using Global;
using Slime.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Slime
{
    public class SimulationInit : Simulation
    {
        [SerializeField] private UnityEvent onSimulationStart;
        [SerializeField] private SpeciesSettings defaultSpeciesSettings;
        private Species[] species;
        private SpeciesSettings _speciesSettings;
        public SpeciesSettings speciesSettings
        {
            get => _speciesSettings;
            set
            {
                _speciesSettings = value;
                species = value.species;
            }
        }
        [SerializeField] public bool useDefaultSpeciesSettings = true;

        [SerializeField] private bool randomizeSpawn = true;
        
        [SerializeField] private EntitySettings entities;
        
        //Cached Shader Parameters
        private static readonly int FoodMap          = Shader.PropertyToID("preFoodMap");
        private static readonly int DiffusedFoodMap  = Shader.PropertyToID("diffusedFoodMap");
        private static readonly int PreTrailMap      = Shader.PropertyToID("preTrailMap");
        private static readonly int DiffusedTrailMap = Shader.PropertyToID("diffusedTrailMap");
        private static readonly int DisplayMap       = Shader.PropertyToID("displayMap");
        private static readonly int DebugMap         = Shader.PropertyToID("debugMap");
        private static readonly int Width            = Shader.PropertyToID("width");
        private static readonly int Height           = Shader.PropertyToID("height");
        private static readonly int AgentAmount      = Shader.PropertyToID("agentAmount");
        private static readonly int FoodColor        = Shader.PropertyToID("foodColor");
        private static readonly int SpeciesAmount    = Shader.PropertyToID("speciesAmount");

        private void Awake()
        {
            Debug.Log("start: " + Time.realtimeSinceStartup);
            
            simulationSettings.generateTextureMaps();
            
            if (useDefaultSpeciesSettings)
            {
                speciesSettings = defaultSpeciesSettings;
            }
            
            startSimulation();
            Debug.Log("done: " + Time.realtimeSinceStartup);
            
        }

        private void Start()
        {
            onSimulationStart.Invoke();
        }

        public void startSimulation()
        {
            if (useDefaultSpeciesSettings)
            {
                speciesSettings = defaultSpeciesSettings;
            }
            
            setupSpecies();
            setupEntities();
            setShaderParameters();
            
            GameState.state = GameState.Simulation;
            
        }

        private void setupSpecies()
        {
            if (randomizeSpawn)
            {
                for (int i = 0; i < species.Length; i++)
                {
                    speciesSettings.randomizeSpawnPosition(i);
                }
            }

            simulationSettings.currentSpeciesAmount = species.Length;
            GraphicsUtility.setupShaderBuffer(ref simulationSettings.speciesBuffer, simulationShader, species, updateKernel, "species");
            simulationShader.SetBuffer(displayKernel, "species", simulationSettings.speciesBuffer);
        }

        private void setupEntities()
        {
            int[] speciesEntities = new int[species.Length];
            Vector2[] spawnPositions = new Vector2[species.Length];
            
            for (int i = 0; i < species.Length; i++)
            {
                speciesEntities[i] = species[i].population;
                spawnPositions[i] = species[i].spawnPosition;
            }
            
            entities.setupEntities(simulationSettings.maxEntityAmount, 
                                   speciesEntities,
                                   spawnPositions,
                                   simulationSettings.spawnRadius);
            
            GraphicsUtility.setupShaderBuffer<Entity>(ref simulationSettings.agentBuffer, simulationShader, entities.entities, updateKernel, "agents");
        }

        private void awakeShader()
        {
            
        }

        private void startShader()
        {
            
        }
        
        //TODO: split into startShader and awakeShader
        private void setShaderParameters()
        {
            simulationShader.SetTexture(updateKernel, FoodMap, simulationSettings.preFoodMap);
            simulationShader.SetTexture(diffuseKernel, FoodMap, simulationSettings.preFoodMap);
            
            simulationShader.SetTexture(updateKernel, DiffusedFoodMap, simulationSettings.diffusedFoodMap);
            simulationShader.SetTexture(diffuseKernel, DiffusedFoodMap, simulationSettings.diffusedFoodMap);
            simulationShader.SetTexture(displayKernel, DiffusedFoodMap, simulationSettings.diffusedFoodMap);
            
            simulationShader.SetTexture(updateKernel, PreTrailMap, simulationSettings.preTrailMap);
            simulationShader.SetTexture(diffuseKernel, PreTrailMap, simulationSettings.preTrailMap);

            simulationShader.SetTexture(diffuseKernel, DiffusedTrailMap, simulationSettings.diffusedTrailMap);
            simulationShader.SetTexture(displayKernel, DiffusedTrailMap, simulationSettings.diffusedTrailMap);

            simulationShader.SetTexture(displayKernel, DisplayMap, simulationSettings.displayMap);

            simulationShader.SetTexture(updateKernel, DebugMap, simulationSettings.debugMap);
            simulationShader.SetTexture(debugKernel, DebugMap, simulationSettings.debugMap);

            simulationShader.SetInt(Width, GameSettings.width);
            simulationShader.SetInt(Height, GameSettings.height);

            simulationShader.SetInt(SpeciesAmount, species.Length);
            simulationShader.SetInt(AgentAmount, simulationSettings.maxEntityAmount);

            simulationShader.SetVector(FoodColor, simulationSettings.foodColor);
        }
    }
}