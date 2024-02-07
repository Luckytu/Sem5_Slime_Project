using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "PowInFast", menuName = "Interpolators/PowInFast", order = 1)]   
    public class PowInFast : Interpolator
    {
        public int strength = 4;
        
        public override float interpolate(float t)
        {
            if (strength % 2 == 0)
            {
                return -Mathf.Pow(t-1, strength) + 1;
            }
            else
            {
                return Mathf.Pow(t-1, strength) + 1;
            }
        }
    }
}