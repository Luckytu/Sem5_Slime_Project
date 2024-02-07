using UnityEngine;
using UnityEngine.Serialization;

namespace Slime
{
    [System.Serializable]
    public struct Species
    {
        public Color color;
        
        public Vector2 spawnPosition;
        public int population;
        public int offset;
        public int alivePopulation;
        public float hungerAccumulation;
        public float interSpeciesHungerModifier;
        
        public float moveSpeed;
        public float turnSpeed;

        public float sensorAngle;
        public float sensorOffset;
    }
    
    public struct Entity
    {
        public int speciesIndex;

        public Vector2 position;
        public float angle;

        public float hunger;
        public int foodPheromoneStorage;
    }
}