using System;
using System.Collections.Generic;
using Slime;
using Slime.Settings;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

namespace SlimeCapture
{
    public class SlimeGraph : MonoBehaviour
    {
        [SerializeField] private SpeciesSettings speciesSettings;

        [SerializeField] private UIDocument uiDocument;
        
        [SerializeField] private ComputeShader graphShader;
        [SerializeField] private RenderTexture[] graphTextures;
        [SerializeField] private int textureAmount = 8;
        
        [SerializeField] private int width = 880;
        [SerializeField] private int height = 200;

        [SerializeField] private int verticalLimit = 300000;

        [SerializeField] private Renderer renderer;
        
        private const int graphKernel = 0;

        private WebCamTexture webCamTexture;
        
        private void Start()
        {
            graphShader.SetInt("width", width);
            graphShader.SetInt("height", height);
            graphShader.SetInt("verticalLimit", verticalLimit);

            graphTextures = new RenderTexture[textureAmount];
            List<Image> graphs = uiDocument.rootVisualElement.Query<Image>("Graph").ToList();

            GraphicsUtility.createRenderTexture(ref graphTextures[0], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[1], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[2], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[3], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[4], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[5], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[6], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            GraphicsUtility.createRenderTexture(ref graphTextures[7], width, height, FilterMode.Point, GraphicsFormat.R16G16B16A16_SFloat);
            
            for (int i = 0; i < textureAmount; i++)
            {
                graphs[i].image = graphTextures[i];
            }
            
            initCaptureCamera();
            renderer.material.mainTexture = webCamTexture;
        }

        private void FixedUpdate()
        {
            if (speciesSettings.species != null)
            {
                for (int i = 0; i < speciesSettings.species.Length; i++)
                {
                    int value = speciesSettings.species[i].alivePopulation - speciesSettings.species[i].population;
                    
                    graphShader.SetTexture(graphKernel, "GraphMap", graphTextures[i]);
                    graphShader.SetInt("verticalLimit", verticalLimit);
                    graphShader.SetInt("value", value);
                    graphShader.SetVector("color", speciesSettings.species[i].color);
                    
                    GraphicsUtility.dispatch(ref graphShader, graphKernel, width, height);
                }
            }
        }
        
        private void initCaptureCamera()
        {
            WebCamDevice[] devices = WebCamTexture.devices;

            if(devices.Length > 0)
            {
                webCamTexture = new WebCamTexture(devices[2].name, 1920 / 5, 1080 / 5, 30);
                webCamTexture.Play();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}