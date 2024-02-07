using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "SquaredOutFast", menuName = "Interpolators/SquaredOutFast", order = 7)]
    public class SquaredOutFast : Interpolator
    {
        public override float interpolate(float t)
        {
            return Mathf.Pow(t-1, 2);
        }
    }
}