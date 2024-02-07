using UnityEngine;

namespace GameFlow.Interpolators
{
    public abstract class Interpolator : ScriptableObject
    {
        public abstract float interpolate(float t);
    }
}