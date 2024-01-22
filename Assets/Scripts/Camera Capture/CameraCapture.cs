using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private RenderTexture cameraMap;
    [SerializeField] private RenderTexture resultMap;

    [Header("Graphics Settings")]
    [SerializeField] private FilterMode filterMode;
    [SerializeField] private GraphicsFormat graphicsFormat;
    [SerializeField][Range(0, 1)] private float filterCutoff;
    [SerializeField] private bool showFiltered;

    [Header("Projection")]
    [SerializeField][Range(0, 1)] private float x1, y1, x2, y2, x3, y3, x4, y4;

    private int cameraCaptureKernel = 0;

    private WebCamTexture webCamTexture;
    private ProjectiveMapping projectiveMapping;

    private void Awake()
    {
        GraphicsUtility.createRenderTexture(ref cameraMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
        GraphicsUtility.createRenderTexture(ref resultMap, GameSettings.width, GameSettings.height, filterMode, graphicsFormat);
    }

    void Start()
    {
        projectiveMapping = GetComponent<ProjectiveMapping>();

        setOnStart();
        initCaptureCamera();
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

            /*
            Image cameraInput = ui.rootVisualElement.Q("CameraInput") as Image;
            Image cameraOutput = ui.rootVisualElement.Q("CameraOutput") as Image;

            cameraInput.image = cameraMap;
            cameraOutput.image = resultMap;
            */
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
        Matrix3x3 A = projectiveMapping.getTransformationMatrix(x1, y1, x2, y2, x3, y3, x4, y4);

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

    public RenderTexture getCameraCapture()
    {
        return resultMap;
    }
}
