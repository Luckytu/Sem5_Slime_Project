using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "PowOutFast", menuName = "Interpolators/PowOutFast", order = 3)]
    public class PowOutFast : Interpolator
    {
        public int strength = 4;
        
        public override float interpolate(float t)
        {
            if (strength % 2 == 0)
            {
                return Mathf.Pow(t-1, strength);
            }
            else
            {
                return -Mathf.Pow(t-1, strength);
            }
        }
    }
}