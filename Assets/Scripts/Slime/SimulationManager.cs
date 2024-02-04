using System;
using Camera_Capture;
using Global;
using Slime.Settings;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Slime
{
    public class SimulationManager : Simulation
    {
        [Header("Graphics Settings")] [SerializeField]
        private bool showDebugMaps = false;
        
        [SerializeField] private SpeciesSettings speciesSettings;
        
        [SerializeField] private CameraCapture cameraCapture;

        [SerializeField] private Transform canvas;
        [SerializeField] private Transform debugCanvas;
        
        private bool _scanFoodMap = false;
        public bool scanFoodMap
        {
            get => _scanFoodMap;
            set
            {
                _scanFoodMap = value;
                simulationShader.SetBool("scanFoodMap", scanFoodMap);
            }
        }
        
        private static readonly int CameraCaptureMap = Shader.PropertyToID("cameraCaptureMap");
        
        // Start is called before the first frame update
        private void Start()
        {
            simulationSettings.cameraCaptureMap = cameraCapture.resultMap;
            simulationShader.SetTexture(diffuseKernel, CameraCaptureMap, simulationSettings.cameraCaptureMap);
            
            canvas.GetComponent<MeshRenderer>().material.mainTexture = simulationSettings.displayMap;
            debugCanvas.GetComponent<MeshRenderer>().material.mainTexture = simulationSettings.diffusedFoodMap;
        }

        private void FixedUpdate()
        {
            if (GameState.state == GameState.Paused)
            {
                return;
            }

            setOnUpdate();
            run();
            debug();
        }

        private void debug()
        {
            if (showDebugMaps)
            {
                debugCanvas.gameObject.SetActive(true);
                GraphicsUtility.dispatch(ref simulationShader, debugKernel, GameSettings.width, GameSettings.height);
            }
            else
            {
                debugCanvas.gameObject.SetActive(false);
            }
        }

        private void run()
        {
            GraphicsUtility.dispatch(ref simulationShader, updateKernel, simulationSettings.maxEntityAmount, 1);
            GraphicsUtility.dispatch(ref simulationShader, diffuseKernel, GameSettings.width, GameSettings.height);
            GraphicsUtility.dispatch(ref simulationShader, displayKernel, GameSettings.width, GameSettings.height);

            Graphics.Blit(simulationSettings.diffusedTrailMap, simulationSettings.preTrailMap);
            Graphics.Blit(simulationSettings.diffusedFoodMap, simulationSettings.preFoodMap);
        }

        private void setOnUpdate()
        {
            //GraphicsUtility.createStructuredBuffer(ref simulationSettings.speciesBuffer, speciesSettings.species);
            //simulationShader.SetBuffer(updateKernel, "species", simulationSettings.speciesBuffer);

            simulationShader.SetFloat("time", Time.fixedTime);
            simulationShader.SetFloat("deltaTime", Time.fixedDeltaTime);

            simulationShader.SetFloat("decayRate", simulationSettings.decayRate);
            simulationShader.SetFloat("diffuseRatio", simulationSettings.diffuseRatio);
            simulationShader.SetFloat("agentContributionRatio", simulationSettings.agentContributionRatio);
            
            simulationShader.SetFloat("minimumPopulationRatio", simulationSettings.minimumPopulationRatio);
            simulationShader.SetFloat("maxFoodPheromoneStorage", simulationSettings.maxFoodPheromoneStorage);

            simulationShader.SetFloat("sampleEntitiesAmount", simulationSettings.sampleEntitiesAmount);
            simulationShader.SetFloat("minSpawnPointDistance", simulationSettings.minSpawnPointDistance);
            simulationShader.SetFloat("spawnPointMoveMultiplier", simulationSettings.spawnPointMoveMultiplier);
            
            simulationShader.SetFloat("randomVariance", simulationSettings.randomVariance);
            simulationShader.SetFloat("deathCutoff", simulationSettings.deathCutoff);
        }
        
        public void updateFoodMap()
        {
            cameraCapture.updateCamera();
        }
        
        private void OnDestroy()
        {
            GraphicsUtility.Release(simulationSettings.agentBuffer, simulationSettings.speciesBuffer);
        }
    }
}