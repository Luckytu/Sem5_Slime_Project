using UnityEngine;

namespace Slime.Settings
{
    [CreateAssetMenu(fileName = "Random Generation Settings", menuName = "Settings/Simulation/Random Generation", order = 0)]
    public class RandomGenerationSettings : ScriptableObject
    {
        public Gradient color;

        public float minPopulationRatio, maxPopulationRatio;

        public float populationDeathCutoffRatio = 0.05f;
        public float minimumLifeTime = 120f;
        public float maximumLifeTime = 1200f;
        
        public float minHungerAccumulation, maxHungerAccumulation;
        public float minInterSpeciesHungerModifier, maxInterSpeciesHungerModifier;

        public float minMoveSpeed, maxMoveSpeed;
        public float minTurnSpeed, maxTurnSpeed;

        public float minSensorAngle, maxSensorAngle;
        public float minSensorOffset, maxSensorOffset;
    }
}