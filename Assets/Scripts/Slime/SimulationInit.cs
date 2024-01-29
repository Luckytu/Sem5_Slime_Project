﻿using System;
using Global;
using Slime.Slime_Settings;
using UnityEngine;
using UnityEngine.Serialization;

namespace Slime
{
    public class SimulationInit : Simulation
    {
        [SerializeField] private SimulationSettings simulationSettings;
        
        [SerializeField] private SpeciesSettings defaultSpeciesSettings;
        public SpeciesSettings.Species[] species;
        public SpeciesSettings _speciesSettings;
        public SpeciesSettings speciesSettings
        {
            get => _speciesSettings;
            set
            {
                _speciesSettings = value;
                species = value.species;
            }
        }
        [SerializeField] private bool useDefaultSpeciesSettings = true;

        [SerializeField] private bool randomizeSpawn = true;
        
        [SerializeField] private EntitySettings entities;
        
        //Cached Shader Parameters
        private static readonly int FoodMap          = Shader.PropertyToID("foodMap");
        private static readonly int PreTrailMap      = Shader.PropertyToID("preTrailMap");
        private static readonly int DiffusedTrailMap = Shader.PropertyToID("diffusedTrailMap");
        private static readonly int DisplayMap       = Shader.PropertyToID("displayMap");
        private static readonly int DebugMap         = Shader.PropertyToID("debugMap");
        private static readonly int Width            = Shader.PropertyToID("width");
        private static readonly int Height           = Shader.PropertyToID("height");
        private static readonly int AgentAmount      = Shader.PropertyToID("agentAmount");
        private static readonly int FoodColor        = Shader.PropertyToID("foodColor");

        private void Awake()
        {
            simulationSettings.generateTextureMaps();
            
            if (useDefaultSpeciesSettings)
            {
                speciesSettings = defaultSpeciesSettings;
            }
        }

        private void Start()
        {
            startSimulation();
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
                    speciesSettings.randomizeSpawnPosition(i, simulationSettings.spawnMargin);
                }
            }
            
            GraphicsUtility.setupShaderBuffer(ref simulationSettings.speciesBuffer, simulationShader, species, updateKernel, "species");
        }

        private void setupEntities()
        {
            int[] speciesEntities = new int[species.Length];
            Vector2[] spawnPositions = new Vector2[species.Length];
            
            for (int i = 0; i < species.Length; i++)
            {
                speciesEntities[i] = species[i].spawns;
                spawnPositions[i] = species[i].spawnPosition;
            }
            
            entities.setupEntities(simulationSettings.maxEntityAmount, 
                                   speciesEntities,
                                   spawnPositions,
                                   simulationSettings.spawnRadius);
            
            GraphicsUtility.setupShaderBuffer<EntitySettings.Entity>(ref simulationSettings.agentBuffer, simulationShader, entities.entities, updateKernel, "agents");
        }
        
        private void setShaderParameters()
        {
            simulationShader.SetTexture(updateKernel, FoodMap, simulationSettings.foodMap);
            simulationShader.SetTexture(displayKernel, FoodMap, simulationSettings.foodMap);

            simulationShader.SetTexture(updateKernel, PreTrailMap, simulationSettings.preTrailMap);
            simulationShader.SetTexture(diffuseKernel, PreTrailMap, simulationSettings.preTrailMap);

            simulationShader.SetTexture(diffuseKernel, DiffusedTrailMap, simulationSettings.diffusedTrailMap);
            simulationShader.SetTexture(displayKernel, DiffusedTrailMap, simulationSettings.diffusedTrailMap);

            simulationShader.SetTexture(displayKernel, DisplayMap, simulationSettings.displayMap);

            simulationShader.SetTexture(updateKernel, DebugMap, simulationSettings.debugMap);
            simulationShader.SetTexture(debugKernel, DebugMap, simulationSettings.debugMap);

            simulationShader.SetInt(Width, GameSettings.width);
            simulationShader.SetInt(Height, GameSettings.height);

            simulationShader.SetInt(AgentAmount, simulationSettings.maxEntityAmount);

            simulationShader.SetVector(FoodColor, simulationSettings.foodColor);
        }
    }
}