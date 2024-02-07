using UnityEngine;

namespace Slime.Settings
{
    [CreateAssetMenu(fileName = "Entity Settings", menuName = "Settings/Simulation/Entity Settings", order = 0)]
    public class EntitySettings : ScriptableObject
    {
        public Entity[] entities;

        public void setupEntities(int entityAmount, int[] speciesEntities, Vector2[] speciesSpawnPositions, float spawnRadius)
        {
            entities = new Entity[entityAmount];
            
            int offset = 0;
            for (int speciesIndex = 0; speciesIndex < speciesEntities.Length; speciesIndex++)
            {
                for (int i = 0; i < speciesEntities[speciesIndex]; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;
                    Vector2 position = speciesSpawnPositions[speciesIndex] + Random.insideUnitCircle * spawnRadius;
                    
                    entities[i + offset] = new Entity()
                    {
                        speciesIndex = speciesIndex,
                        
                        position = position,
                        angle = angle,
                        
                        hunger = 0,
                        foodPheromoneStorage = 0 
                    };
                }

                offset += speciesEntities[speciesIndex];
            }
        }
    }
}