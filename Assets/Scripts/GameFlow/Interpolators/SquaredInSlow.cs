using UnityEngine;

namespace GameFlow.Interpolators
{
    [CreateAssetMenu(fileName = "SquaredInSlow", menuName = "Interpolators/SquaredInSlow", order = 6)]
    public class SquaredInSlow : Interpolator
    {
        public override float interpolate(float t)
        {
            return Mathf.Pow(t, 2);
        }
    }
}