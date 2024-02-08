using UnityEngine;
using UnityEngine.Serialization;

namespace Slime.Settings
{
    [CreateAssetMenu(fileName = "Effects Settings", menuName = "Settings/Simulation/Effects Settings", order = 0)]
    public class EffectsSettings : ScriptableObject
    {
        public float maxPixelSize;

        public int maxAreasAmount;

        public int minAreaX;
        public int maxAreaX;
        public int growthX;

        public int minAreaY;
        public int maxAreaY;
        public int growthY;

        public float minDistort = 0.17f;
        public float maxDistort = 2.5f;
    }
}