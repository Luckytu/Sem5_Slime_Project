using ComputeShaderUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private RenderTexture displayTexture;

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

    // Start is called before the first frame update
    void Start()
    {
        shaderSetup();

        createRenderTexture(ref displayTexture, width, height, filterMode, format);

        transform.GetComponentInChildren<MeshRenderer>().material.mainTexture = displayTexture;

        Vector3Int threadGroupSizes = getThreadGroupSizes(simulationShader, 0);
        int numGroupsX = Mathf.CeilToInt(width / (float)threadGroupSizes.x);
        int numGroupsY = Mathf.CeilToInt(height / (float)threadGroupSizes.y);

        simulationShader.Dispatch(0, numGroupsX, numGroupsY, 1);
    }

    private void shaderSetup()
    {
        simulationShader.SetTexture(0, "ColorMap", displayTexture);
        simulationShader.SetInt("width", width);
        simulationShader.SetInt("height", height);

        simulationShader.SetInt("agentsAmount", agentsAmount);
        simulationShader.SetFloat("decayRate", decayRate);
    }

    // Update is called once per frame
    void Update()
    {
        
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
