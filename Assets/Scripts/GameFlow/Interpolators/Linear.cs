using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "Linear", menuName = "Interpolators/Linear", order = 0)]
    public class Linear : Interpolator
    {
        public override float interpolate(float t)
        {
            return t;
        }
    }
}