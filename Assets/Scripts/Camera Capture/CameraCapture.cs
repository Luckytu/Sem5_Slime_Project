using System;
using Global;
using Slime;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

namespace Camera_Capture
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(ProjectiveMapping))]

    public class CameraCapture : MonoBehaviour
    {
        [SerializeField] private CameraCaptureSettings settings;
    
        [Header("References")]
        [SerializeField] private ComputeShader cameraCaptureShader;
        [SerializeField] private SimulationManager simulation;
        [SerializeField] private UIDocument ui;
        [SerializeField] public RenderTexture cameraMap;
        [SerializeField] public RenderTexture resultMap;

        [Header("Graphics Settings")]
        [SerializeField] private FilterMode filterMode;
        [SerializeField] private GraphicsFormat graphicsFormat;
        [SerializeField] private bool showFiltered;
        [SerializeField] private bool updateConstantly;

        private MeshRenderer rend;
    
        private const int cameraCaptureKernel = 0;

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

            rend = GetComponent<MeshRenderer>();
            rend.material.mainTexture = resultMap;
            rend.enabled = false;
        
            hide();
        }

        private void FixedUpdate()
        {
            if (updateConstantly)
            {
                updateCamera();
            }
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
            Matrix3x3 A = projectiveMapping.getTransformationMatrix
            (settings.p.x1, settings.p.y1, 
                settings.p.x2, settings.p.y2, 
                settings.p.x3, settings.p.y3, 
                settings.p.x4, settings.p.y4);

            cameraCaptureShader.SetFloat("a11", A.a11);
            cameraCaptureShader.SetFloat("a12", A.a12);
            cameraCaptureShader.SetFloat("a13", A.a13);

            cameraCaptureShader.SetFloat("a21", A.a21);
            cameraCaptureShader.SetFloat("a22", A.a22);
            cameraCaptureShader.SetFloat("a23", A.a23);

            cameraCaptureShader.SetFloat("a31", A.a31);
            cameraCaptureShader.SetFloat("a32", A.a32);

            cameraCaptureShader.SetFloat("filterCutoff", settings.filterCutoff);
            cameraCaptureShader.SetBool("showFiltered", showFiltered);
        }

        public CameraCaptureSettings.EdgePointCoords getEdgePoints()
        {
            return settings.p;
        }

        public void setEdgePoints(String edgePointName, float x, float y)
        {
            switch (edgePointName)
            {
                case "x1":
                    settings.p.x1 = x; settings.p.y1 = y;
                    break;
                case "x2":
                    settings.p.x2 = x; settings.p.y2 = y;
                    break;
                case "x3":
                    settings.p.x3 = x; settings.p.y3 = y;
                    break;
                case "x4":
                    settings.p.x4 = x; settings.p.y4 = y;
                    break;
                default:
                    return;
            }
        }

        public void updateCamera()
        {
            setOnUpdate();

            Graphics.Blit(webCamTexture, cameraMap);
            GraphicsUtility.dispatch(ref cameraCaptureShader, cameraCaptureKernel, GameSettings.width, GameSettings.height);
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
}
