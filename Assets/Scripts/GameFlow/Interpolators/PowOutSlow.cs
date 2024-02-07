using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "PowOutSlow", menuName = "Interpolators/PowOutSlow", order = 4)]
    public class PowOutSlow : Interpolator
    {
        public int strength = 4;
        
        public override float interpolate(float t)
        {
            return -Mathf.Pow(t, strength) + 1;
        }
    }
}