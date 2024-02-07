using Global;
using UnityEngine;

namespace Slime.Settings
{
    [CreateAssetMenu(fileName = "Species Settings", menuName = "Settings/Simulation/Species Settings", order = 0)]
    public class SpeciesSettings : ScriptableObject
    {
        public float spawnMargin;
        
        public Species[] species;

        public void randomizeSpawnPosition(int speciesIndex)
        {
            if (speciesIndex < 0 || speciesIndex >= species.Length)
            {
                return;
            }
            
            float x = Random.Range(GameSettings.width * spawnMargin, GameSettings.width * (1 - spawnMargin));
            float y = Random.Range(GameSettings.height * spawnMargin, GameSettings.height * (1 - spawnMargin));
            
            species[speciesIndex].spawnPosition = new Vector2(x, y);
        }

        public Vector2 getRandomSpawnPosition()
        {
            float x = Random.Range(GameSettings.width * spawnMargin, GameSettings.width * (1 - spawnMargin));
            float y = Random.Range(GameSettings.height * spawnMargin, GameSettings.height * (1 - spawnMargin));

            return new Vector2(x, y);
        }
        
        public static Vector2 getRandomSpawnPosition(float margin)
        {
            float x = Random.Range(GameSettings.width * margin, GameSettings.width * (1 - margin));
            float y = Random.Range(GameSettings.height * margin, GameSettings.height * (1 - margin));

            return new Vector2(x, y);
        }
        
        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}