using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "PowInSlow", menuName = "Interpolators/PowInSlow", order = 2)]
    public class PowInSlow : Interpolator
    {
        public int strength = 4;
        
        public override float interpolate(float t)
        {
            return Mathf.Pow(t, strength);
        }
    }
}