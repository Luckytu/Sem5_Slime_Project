using System;
using Global;
using Slime.Slime_Settings;
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

        [SerializeField] private SimulationSettings simulationSettings;
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

        // Start is called before the first frame update
        private void Start()
        {
            updateFoodMap();

            canvas.GetComponent<MeshRenderer>().material.mainTexture = simulationSettings.displayMap;
            debugCanvas.GetComponent<MeshRenderer>().material.mainTexture = simulationSettings.debugMap;
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
        }

        private void setOnUpdate()
        {
            GraphicsUtility.createStructuredBuffer(ref simulationSettings.speciesBuffer, speciesSettings.species);
            simulationShader.SetBuffer(updateKernel, "species", simulationSettings.speciesBuffer);

            simulationShader.SetFloat("time", Time.fixedTime);
            simulationShader.SetFloat("deltaTime", Time.fixedDeltaTime);

            simulationShader.SetFloat("globalSpeed", speciesSettings.species[0].moveSpeed);
            simulationShader.SetFloat("globalTurnSpeed", speciesSettings.species[0].turnSpeed);
            simulationShader.SetFloat("globalSensorAngle", speciesSettings.species[0].sensorAngle);
            simulationShader.SetFloat("globalSensorOffset", speciesSettings.species[0].sensorOffset);

            simulationShader.SetFloat("decayRate", simulationSettings.decayRate);
            simulationShader.SetFloat("diffuseRatio", simulationSettings.diffuseRatio);
            simulationShader.SetFloat("agentContributionRatio", simulationSettings.agentContributionRatio);
            simulationShader.SetFloat("randomVariance", simulationSettings.randomVariance);
            simulationShader.SetFloat("randomDeath", simulationSettings.deathCutoff);
        }
        
        public void updateFoodMap()
        {
            Graphics.Blit(cameraCapture.resultMap, simulationSettings.foodMap);
        }
        
        private void OnDestroy()
        {
            GraphicsUtility.Release(simulationSettings.agentBuffer, simulationSettings.speciesBuffer);
        }
    }
}