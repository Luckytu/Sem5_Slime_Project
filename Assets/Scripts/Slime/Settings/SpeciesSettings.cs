using Global;
using UnityEngine;

namespace Slime.Settings
{
    [CreateAssetMenu(fileName = "Species Settings", menuName = "Settings/Simulation/Species Settings", order = 0)]
    public class SpeciesSettings : ScriptableObject
    {
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