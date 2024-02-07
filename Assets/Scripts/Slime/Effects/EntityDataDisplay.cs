using System;
using Global;
using Slime.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Slime.Effects
{
    public class EntityDataDisplay : MonoBehaviour
    {
        public struct LabelElements
        {
            public VisualElement parent;
            public Label speciesIndex;
            public Label position;
            public Label angle;
            public Label hunger;
            public Label foodPheromoneStorage;
        }
        
        private LabelElements[] labelElements;

        [SerializeField] private SimulationSettings simulationSettings;
        [SerializeField] private UIDocument uiDocument;

        [SerializeField] private VisualTreeAsset dataDisplaySource;
        private TemplateContainer[] dataDisplays;

        [SerializeField] [Range(0, 1)] private float errorMessageMultiplier;
        
        public Entity[] entities { private get; set; }
        
        private VisualElement root;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;
        }

        private void Update()
        {
            if (entities == null || GameState.state != GameState.Simulation)
            {
                return;
            }

            for (int i = 0; i < entities.Length; i++)
            {
                float xCoord = entities[i].position.x;
                float yCoord = GameSettings.height - entities[i].position.y;
                
                string x = xCoord.ToString("N2");
                string y = yCoord.ToString("N2");

                
                labelElements[i].parent.style.left = xCoord;
                labelElements[i].parent.style.top = yCoord;

                if (entities[i].hunger > simulationSettings.deathCutoff * errorMessageMultiplier)
                {
                    labelElements[i].speciesIndex.text = "DEATH IMMINENT";
                    labelElements[i].speciesIndex.style.color = simulationSettings.foodColor;
                    labelElements[i].position.text = "";
                    labelElements[i].angle.text = "";
                    labelElements[i].hunger.text = "";
                    labelElements[i].foodPheromoneStorage.text = "";
                }
                else
                {
                    labelElements[i].speciesIndex.text = "ALIVE";
                    labelElements[i].speciesIndex.style.color = Color.white;
                    labelElements[i].position.text = "(" + x + " " + y + ")";
                    labelElements[i].angle.text = entities[i].angle.ToString("N2");
                    labelElements[i].hunger.text = entities[i].hunger.ToString("N2");
                    labelElements[i].foodPheromoneStorage.text = entities[i].foodPheromoneStorage.ToString("N2");
                }
            }
        }

        public void instantiateDataDisplays(int entitiesAmount)
        {
            entities = new Entity[entitiesAmount];
            dataDisplays = new TemplateContainer[entitiesAmount];
            labelElements = new LabelElements[entitiesAmount];
            
            for(int i = 0; i < entitiesAmount; i++)
            {
                dataDisplays[i] = dataDisplaySource.Instantiate();
            
                labelElements[i] = new LabelElements()
                {
                    parent               = dataDisplays[i].Q("DataContainer"),
                    speciesIndex         = dataDisplays[i].Q("SpeciesIndex")         as Label,
                    position             = dataDisplays[i].Q("Position")             as Label,
                    angle                = dataDisplays[i].Q("Angle")                as Label,
                    hunger               = dataDisplays[i].Q("Hunger")               as Label,
                    foodPheromoneStorage = dataDisplays[i].Q("FoodPheromoneStorage") as Label,
                };
                            
                root.Add(dataDisplays[i]);
            }
        }
        
        public void toggleVisible()
        {
            root.visible = !root.visible;
        }
    }
}