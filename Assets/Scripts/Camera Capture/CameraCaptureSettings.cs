using System;
using UnityEngine;

namespace Camera_Capture
{
    [CreateAssetMenu(fileName = "Camera Capture Settings", menuName = "Settings/Camera Capture", order = 0)]
    public class CameraCaptureSettings : ScriptableObject
    {
        [System.Serializable]
        public struct EdgePointCoords
        {
            public float x1, y1;
            public float x2, y2;
            public float x3, y3;
            public float x4, y4;
        }
        public EdgePointCoords p;

        public float filterCutoff;

        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}