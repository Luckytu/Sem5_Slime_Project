using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "SquaredOutSlow", menuName = "Interpolators/SquaredOutSlow", order = 8)]
    public class SquaredOutSlow : Interpolator
    {
        public override float interpolate(float t)
        {
            return -Mathf.Pow(t, 2) + 1;
        }
    }
}