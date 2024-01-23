using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class SimulationManager : MonoBehaviour
{
    [Header("Graphics Settings")]
    [SerializeField] private bool showDebugMaps = false;

    [SerializeField] private FilterMode filterMode = FilterMode.Point;
    [SerializeField] private GraphicsFormat graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

    [SerializeField] private RenderTexture foodMap;
    [SerializeField] private RenderTexture preTrailMap;
    [SerializeField] private RenderTexture diffusedTrailMap;

    [SerializeField] private RenderTexture displayMap;

    [SerializeField] private RenderTexture debugMap;

    [SerializeField] private CameraCapture cameraCapture;

    [SerializeField] private Transform canvas;
    [SerializeField] private Transform debugCanvas;

    [SerializeField] private int width = 1920;
    [SerializeField] private int height = 1080;

    [SerializeField] private Color backgroundColor = Color.black;

    [Header("Shader Settings")]
    [SerializeField] private ComputeShader simulationShader;

    [Header("Slime Settings")]
    [SerializeField] private int agentAmount;
    [SerializeField] private int speciesAmount;
    [SerializeField] private Gradient speciesColorRange;

    [SerializeField] private float decayRate;
    [SerializeField] private float diffuseRatio;
    [SerializeField] private float agentContributionRatio;
    [SerializeField] private float randomVariance;
    [SerializeField] private float randomDeath;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float sensorAngle;
    [SerializeField] private float sensorOffset;

    private const int updateKernel = 0;
    private const int diffuseKernel = 1;
    private const int displayKernel = 2;
    private const int debugKernel = 3;

    private ComputeBuffer agentBuffer;
    private ComputeBuffer speciesBuffer;

    private Species[] species;
    private Vector2[] spawnPositions;

    public struct Agent
    {
        public int speciesIndex;

        public Vector2 position;
        public float angle;

        public float hunger;
    }

    public struct Species
    {
        public Color color;
        public Vector2 spawnPosition;

        public float moveSpeed;
        public float turnSpeed;

        public float sensorAngle;
        public float sensorOffset;
    }

    // Start is called before the first frame update
    private void Start()
    {
        GraphicsUtility.createRenderTexture(ref preTrailMap, width, height, filterMode, graphicsFormat);
        GraphicsUtility.createRenderTexture(ref diffusedTrailMap, width, height, filterMode, graphicsFormat);
        GraphicsUtility.createRenderTexture(ref displayMap, width, height, filterMode, graphicsFormat);

        GraphicsUtility.createRenderTexture(ref debugMap, width, height, filterMode, graphicsFormat);

        foodMap = cameraCapture.resultMap;

        setSpecies();
        setAgents();
        setOnStart();

        canvas.GetComponent<MeshRenderer>().material.mainTexture = displayMap;
        debugCanvas.GetComponent<MeshRenderer>().material.mainTexture = debugMap;
    }

    private void FixedUpdate()
    {
        setOnUpdate();
        run();
        debug();
    }

    private void debug()
    {
        if (showDebugMaps)
        {
            debugCanvas.gameObject.SetActive(true);
            GraphicsUtility.dispatch(ref simulationShader, debugKernel, width, height);
        }
        else
        {
            debugCanvas.gameObject.SetActive(false);
        }
    }

    private void run()
    {
        GraphicsUtility.dispatch(ref simulationShader, updateKernel, agentAmount, 1);
        GraphicsUtility.dispatch(ref simulationShader, diffuseKernel, width, height);
        GraphicsUtility.dispatch(ref simulationShader, displayKernel, width, height);

        Graphics.Blit(diffusedTrailMap, preTrailMap);
    }

    private void setOnUpdate()
    {
        GraphicsUtility.createStructuredBuffer(ref speciesBuffer, species);
        simulationShader.SetBuffer(updateKernel, "species", speciesBuffer);

        simulationShader.SetFloat("time", Time.fixedTime);
        simulationShader.SetFloat("deltaTime", Time.fixedDeltaTime);

        simulationShader.SetFloat("globalSpeed", moveSpeed);
        simulationShader.SetFloat("globalTurnSpeed", turnSpeed);
        simulationShader.SetFloat("globalSensorAngle", sensorAngle);
        simulationShader.SetFloat("globalSensorOffset", sensorOffset);

        simulationShader.SetFloat("decayRate", decayRate);
        simulationShader.SetFloat("diffuseRatio", diffuseRatio);
        simulationShader.SetFloat("agentContributionRatio", agentContributionRatio);
        simulationShader.SetFloat("randomVariance", randomVariance);
        simulationShader.SetFloat("randomDeath", randomDeath);
    }

    private void setOnStart()
    {
        simulationShader.SetTexture(updateKernel, "foodMap", foodMap);
        simulationShader.SetTexture(displayKernel, "foodMap", foodMap);

        simulationShader.SetTexture(updateKernel, "preTrailMap", preTrailMap);
        simulationShader.SetTexture(diffuseKernel, "preTrailMap", preTrailMap);

        simulationShader.SetTexture(diffuseKernel, "diffusedTrailMap", diffusedTrailMap);
        simulationShader.SetTexture(displayKernel, "diffusedTrailMap", diffusedTrailMap);
        
        simulationShader.SetTexture(displayKernel, "displayMap", displayMap);

        simulationShader.SetTexture(updateKernel, "debugMap", debugMap);
        simulationShader.SetTexture(debugKernel, "debugMap", debugMap);

        simulationShader.SetInt("width", width);
        simulationShader.SetInt("height", height);

        simulationShader.SetInt("agentAmount", agentAmount);

        simulationShader.SetVector("backgroundColor", backgroundColor);
    }

    private void setSpecies()
    {
        species = new Species[speciesAmount];
        spawnPositions = new Vector2[speciesAmount];

        for(int i = 0; i < species.Length; i++)
        {
            Color color = speciesColorRange.Evaluate(Mathf.Clamp01((float)i / (float)(speciesAmount - 1)));
            
            float x = Random.Range(width * 0.1f, width * 0.9f);
            float y = Random.Range(height * 0.1f, height * 0.9f);

            spawnPositions[i] = new Vector2(x, y);

            species[i] = new Species()
            {
                color = color,

                spawnPosition = spawnPositions[i],

                moveSpeed = this.moveSpeed,
                turnSpeed = this.turnSpeed,

                sensorAngle = this.sensorAngle,
                sensorOffset = this.sensorOffset
            };
        }

        GraphicsUtility.setupShaderBuffer(ref speciesBuffer, simulationShader, species, updateKernel, "species");
    }

    private void setAgents()
    {
        Agent[] agents = new Agent[agentAmount];

        float[] spawnRadi = new float[speciesAmount];

        for(int i = 0; i < speciesAmount; i++)
        {
            spawnRadi[i] = Random.Range(height * 0.01f, height * 0.08f);
        }

        for(int i = 0; i < agents.Length; i++)
        {
            float angle = Random.value * Mathf.PI * 2;

            int speciesIndex = i % speciesAmount;
            Vector2 position = spawnPositions[speciesIndex] + Random.insideUnitCircle * spawnRadi[speciesIndex];

            agents[i] = new Agent() 
            {
                speciesIndex = speciesIndex, 
                position = position, 
                angle = angle,
                hunger = 0
            };
        }

        GraphicsUtility.setupShaderBuffer<Agent>(ref agentBuffer, simulationShader, agents, updateKernel, "agents");
    }

    private void OnDestroy()
    {
        GraphicsUtility.Release(agentBuffer, speciesBuffer);
    }


}
