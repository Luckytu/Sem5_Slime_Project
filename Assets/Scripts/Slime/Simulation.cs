using System;
using Global;
using Slime.Slime_Settings;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Slime
{
    public abstract class Simulation : MonoBehaviour
    {
        [Header("Texture Maps")]
        [SerializeField] protected RenderTexture foodMap;
        [SerializeField] protected RenderTexture preTrailMap;
        [SerializeField] protected RenderTexture diffusedTrailMap;
        [SerializeField] protected RenderTexture displayMap;
        [SerializeField] protected RenderTexture debugMap;
        
        [Header("Shader")]
        [SerializeField] protected ComputeShader simulationShader;
        
        protected const int updateKernel = 0;
        protected const int diffuseKernel = 1;
        protected const int displayKernel = 2;
        protected const int debugKernel = 3;
    }
}