using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(ProjectiveMapping))]

public class CameraCapture : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ComputeShader cameraCaptureShader;
    [SerializeField] private SimulationManager simulation;
    [SerializeField] private UIDocument ui;
    [SerializeField] public RenderTexture cameraMap;
    [SerializeField] public RenderTexture resultMap;

    [Header("Graphics Settings")]
    [SerializeField] private FilterMode filterMode;
    [SerializeField] private GraphicsFormat graphicsFormat;
    [SerializeField] public float filterCutoff { get; set; }
    [SerializeField] private bool showFiltered;

    private MeshRenderer rend;

    public struct EdgePointCoords
    {
        public float x1, y1;
        public float x2, y2;
        public float x3, y3;
        public float x4, y4;
    }
    private EdgePointCoords p;

    private int cameraCaptureKernel = 0;

    private WebCamTexture webCamTexture;
    private ProjectiveMapping projectiveMapping;

    private void Awake()
    {
        GraphicsUtility.createRenderTexture(ref cameraMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
        GraphicsUtility.createRenderTexture(ref resultMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);

        p = new EdgePointCoords()
        {
            x1 = 0.302f, y1 = 0.277f,
            x2 = 0.818f, y2 = 0.264f,
            x3 = 0.805f, y3 = 0.742f,
            x4 = 0.327f, y4 = 0.799f
        };
    }

    void Start()
    {
        projectiveMapping = GetComponent<ProjectiveMapping>();

        setOnStart();
        initCaptureCamera();

        rend = GetComponent<MeshRenderer>();
        rend.material.mainTexture = resultMap;
        rend.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            setOnUpdate();
        }
        setOnUpdate();

        GraphicsUtility.dispatch(ref cameraCaptureShader, cameraCaptureKernel, GameSettings.width, GameSettings.height);
        Graphics.Blit(webCamTexture, cameraMap);
    }

    private void initCaptureCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        foreach(WebCamDevice device in devices)
        {
            Debug.Log(device.name);
        }

        if(devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[1].name, 1920 / 5, 1080 / 5, 30);
            webCamTexture.Play();
        }
        else
        {
            throw new ArgumentNullException();
        }
    }

    private void setOnStart()
    {
        cameraCaptureShader.SetTexture(cameraCaptureKernel, "cameraMap", cameraMap);
        cameraCaptureShader.SetTexture(cameraCaptureKernel, "resultMap", resultMap);

        cameraCaptureShader.SetInt("width", GameSettings.width);
        cameraCaptureShader.SetInt("height", GameSettings.height);
    }

    private void setOnUpdate()
    {
        Matrix3x3 A = projectiveMapping.getTransformationMatrix(p.x1, p.y1, p.x2, p.y2, p.x3, p.y3, p.x4, p.y4);

        cameraCaptureShader.SetFloat("a11", A.a11);
        cameraCaptureShader.SetFloat("a12", A.a12);
        cameraCaptureShader.SetFloat("a13", A.a13);

        cameraCaptureShader.SetFloat("a21", A.a21);
        cameraCaptureShader.SetFloat("a22", A.a22);
        cameraCaptureShader.SetFloat("a23", A.a23);

        cameraCaptureShader.SetFloat("a31", A.a31);
        cameraCaptureShader.SetFloat("a32", A.a32);

        cameraCaptureShader.SetFloat("filterCutoff", filterCutoff);
        cameraCaptureShader.SetBool("showFiltered", showFiltered);
    }

    public EdgePointCoords getEdgePoints()
    {
        return p;
    }

    public void setEdgePoints(String edgePointName, float x, float y)
    {
        switch (edgePointName)
        {
            case "x1":
                p.x1 = x; p.y1 = y;
                break;
            case "x2":
                p.x2 = x; p.y2 = y;
                break;
            case "x3":
                p.x3 = x; p.y3 = y;
                break;
            case "x4":
                p.x4 = x; p.y4 = y;
                break;
            default:
                return;
        }
    }

    public void show()
    {
        rend.enabled = true;
    }

    public void hide()
    {
        rend.enabled = false;
    }
}
