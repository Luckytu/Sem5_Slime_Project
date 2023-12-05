using ComputeShaderUtility;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System.Runtime.InteropServices;

public class SimulationManager : MonoBehaviour
{
    private const int updateKernel = 0;
    private const int colorKernel = 1;

    [SerializeField] private RenderTexture displayTexture;
    [SerializeField] private RenderTexture trailMap;
    [SerializeField] private RenderTexture diffuseMap;
    [SerializeField] private RenderTexture foodMap;

    [Header("Simulation Shader")]
    [SerializeField] private ComputeShader simulationShader;

    [Header("Display Settings")]
    [SerializeField] private FilterMode filterMode = FilterMode.Point;
    [SerializeField] private GraphicsFormat format = ComputeHelper.defaultGraphicsFormat;

    [Header("Simulation Settings")]
    [SerializeField] private int width = 192;
    [SerializeField] private int height = 108;

    [Header("Slime Settings")]
    [SerializeField] private int agentsAmount = 100;
    [SerializeField] private float decayRate = 0.5f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float turnSpeed = 0.5f;
    [SerializeField] private Gradient possibleSpeciesGradient;
    [SerializeField] private int speciesAmount = 1;

    [SerializeField] private float sensorAngle = Mathf.PI;
    [SerializeField] private float sensorOffset = 5f;

    private ComputeBuffer settingsBuffer;

    public struct SpeciesSettings
    {
        public float moveSpeed;
        public float turnSpeed;
        public Color color;

        public float sensorAngle;
        public float sensorOffset;
    }

    private SpeciesSettings[] speciesSettings;

    public struct Agent
    {
        public int species;

        public Vector2 position;
        public float angle;
    }

    private Agent[] agents;

    // Start is called before the first frame update
    void Start()
    {
        //createRenderTexture(ref displayTexture, width, height, filterMode, format);
        createRenderTexture(ref trailMap, width, height, filterMode, format);
        createRenderTexture(ref diffuseMap, width, height, filterMode, format);

        transform.GetComponentInChildren<MeshRenderer>().material.mainTexture = diffuseMap;

        setupSpecies();
        spawnAgents();

        setupShader();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        simulationShader.SetFloat("deltaTime", Time.deltaTime);

        simulationShader.SetBuffer(updateKernel, "speciesSettings", settingsBuffer);
        simulationShader.SetBuffer(colorKernel, "speciesSettings", settingsBuffer);

        dispatch(updateKernel);
        dispatch(colorKernel);
    }

    private void dispatch(int kernelID)
    {
        Vector3Int threadGroupSizes = getThreadGroupSizes(simulationShader, kernelID);
        int numGroupsX = Mathf.CeilToInt(width / (float)threadGroupSizes.x);
        int numGroupsY = Mathf.CeilToInt(height / (float)threadGroupSizes.y);

        simulationShader.Dispatch(kernelID, numGroupsX, numGroupsY, 1);
    }

    private void spawnAgents()
    {
        agents = new Agent[agentsAmount];

        for (int i = 0; i < agentsAmount; i++)
        {
            Vector2 position = new Vector2(width / 2f, height / 2f);
            float angle = Random.value * Mathf.PI * 2;

            agents[i] = new Agent() { 
                species = Random.Range(0, speciesAmount),

                position = position, 
                angle = angle
            };
        }

        setupShaderBuffer<Agent>(simulationShader, agents, updateKernel, "agents");
    }

    private void setupShader()
    {
        simulationShader.SetTexture(updateKernel, "trailMap", trailMap);
        simulationShader.SetTexture(colorKernel, "trailMap", trailMap);
        simulationShader.SetTexture(updateKernel, "diffuseMap", diffuseMap);
        simulationShader.SetTexture(colorKernel, "diffuseMap", diffuseMap);
        simulationShader.SetInt("width", width);
        simulationShader.SetInt("height", height);

        simulationShader.SetInt("agentsAmount", agentsAmount);
        simulationShader.SetFloat("decayRate", decayRate);
        simulationShader.SetFloat("moveSpeed", moveSpeed);
    }

    private void setupSpecies()
    {
        speciesSettings = new SpeciesSettings[speciesAmount];

        for(int i = 0; i < speciesAmount; i++)
        {
            speciesSettings[i] = new SpeciesSettings()
            {
                moveSpeed = this.moveSpeed,
                turnSpeed = this.turnSpeed,
                color = possibleSpeciesGradient.Evaluate(Random.value),
                sensorAngle = this.sensorAngle,
                sensorOffset = this.sensorOffset
            };
            Debug.Log(speciesSettings[i].color);
        }

        settingsBuffer = createShaderBuffer<SpeciesSettings>(speciesSettings);
        simulationShader.SetBuffer(updateKernel, "speciesSettings", settingsBuffer);
        simulationShader.SetBuffer(colorKernel, "speciesSettings", settingsBuffer);
    }

    public static ComputeBuffer createShaderBuffer<T>(T[] bufferData)
    {
        ComputeBuffer computeBuffer = new ComputeBuffer(bufferData.Length, Marshal.SizeOf(typeof(T)));
        computeBuffer.SetData(bufferData);
        return computeBuffer;
    }

    public static void setupShaderBuffer<T>(ComputeShader computeShader, T[] bufferData, int kernel, string bufferName)
    {
        ComputeBuffer computeBuffer = new ComputeBuffer(bufferData.Length, Marshal.SizeOf(typeof(T)));
        computeBuffer.SetData(bufferData);
        computeShader.SetBuffer(kernel, bufferName, computeBuffer);
    }

    public static Vector3Int getThreadGroupSizes(ComputeShader compute, int kernelIndex = 0)
    {
        uint x, y, z;
        compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
        return new Vector3Int((int)x, (int)y, (int)z);
    }

    //creates a RenderTexture
    public static void createRenderTexture(ref RenderTexture texture, int width, int height, FilterMode filterMode, GraphicsFormat format)
    {
        if (texture == null || !texture.IsCreated() || texture.width != width || texture.height != height || texture.graphicsFormat != format)
        {
            if (texture != null)
            {
                texture.Release();
            }
            texture = new RenderTexture(width, height, 0);
            texture.graphicsFormat = format;
            texture.enableRandomWrite = true;

            texture.autoGenerateMips = false;
            texture.Create();
        }
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = filterMode;
    }
}
