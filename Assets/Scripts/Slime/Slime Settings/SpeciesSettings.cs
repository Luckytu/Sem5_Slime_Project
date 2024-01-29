using Global;
using UnityEngine;

namespace Slime.Slime_Settings
{
    [CreateAssetMenu(fileName = "Species Settings", menuName = "Settings/Simulation/Species Settings", order = 0)]
    public class SpeciesSettings : ScriptableObject
    {
        [System.Serializable]
        public struct Species
        {
            public Color color;
            
            public Vector2 spawnPosition;
            public int spawns;
            public int availableSpawns;
            public float hungerAccumulation;
            public float interSpeciesHungerModifier;
            
            public float moveSpeed;
            public float turnSpeed;

            public float sensorAngle;
            public float sensorOffset;
        }

        public Species[] species;

        public void randomizeSpawnPosition(int speciesIndex, float margin)
        {
            if (speciesIndex < 0 || speciesIndex >= species.Length)
            {
                return;
            }
            
            float x = Random.Range(GameSettings.width * margin, GameSettings.width * (1 - margin));
            float y = Random.Range(GameSettings.height * margin, GameSettings.height * (1 - margin));
            
            species[speciesIndex].spawnPosition = new Vector2(x, y);
        }
        
        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}