using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "SquaredInFast", menuName = "Interpolators/SquaredInFast", order = 5)]
    public class SquaredInFast : Interpolator
    {
        public override float interpolate(float t)
        {
            return -Mathf.Pow(t-1, 2) + 1;
        }
    }
}