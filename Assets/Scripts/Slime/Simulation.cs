using System;
using Global;
using Slime.Settings;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Slime
{
    public abstract class Simulation : MonoBehaviour
    {
        [Header("Shader")]
        [SerializeField] protected ComputeShader simulationShader;
        [SerializeField] protected SimulationSettings simulationSettings;
        
        protected const int updateKernel = 0;
        protected const int diffuseKernel = 1;
        protected const int displayKernel = 2;
        protected const int debugKernel = 3;
    }
}