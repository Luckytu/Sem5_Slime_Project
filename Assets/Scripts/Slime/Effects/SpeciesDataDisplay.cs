using System;
using System.Linq;
using Global;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Slime.Effects
{
    public class SpeciesDataDisplay : MonoBehaviour
    {
        public struct LabelElements
        {
            public VisualElement parent;
            public Label color;
            public Label spawnPosition;
            public Label population;
            public Label alivePopulation;
            public Label hungerAccumulation;
            public Label turnSpeed;
            public Label sensorAngle;
            public Label sensorAngleOffset;
        }

        private LabelElements[] labelElements;
            
        [SerializeField] private UIDocument uiDocument;

        [SerializeField] private VisualTreeAsset dataDisplaySource;
        private TemplateContainer[] dataDisplays;
        
        public Species[] species { private get; set; }
        
        private VisualElement root;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;
        }

        private void Update()
        {
            if (species == null || GameState.state != GameState.Simulation)
            {
                return;
            }
            
            for(int i = 0; i < species.Length; i++)
            {
                float xCoord = species[i].spawnPosition.x;
                float yCoord = GameSettings.height - species[i].spawnPosition.y;
                
                string x = xCoord.ToString("N2");
                string y = yCoord.ToString("N2");

                labelElements[i].spawnPosition.text = "(" + x + " " + y + ")";
                labelElements[i].alivePopulation.text = species[i].alivePopulation.ToString("N0");
                
                labelElements[i].parent.style.left = xCoord;
                labelElements[i].parent.style.top = yCoord;
                
                string r = species[i].color.r.ToString("N2");
                string g = species[i].color.g.ToString("N2");
                string b = species[i].color.b.ToString("N2");

                labelElements[i].color.text = "(" + r + " " + g + " " + b + ")";
                labelElements[i].population.text = species[i].population.ToString("N0");
                labelElements[i].hungerAccumulation.text = species[i].hungerAccumulation.ToString("N2");
                labelElements[i].turnSpeed.text = species[i].turnSpeed.ToString("N2");
                labelElements[i].sensorAngle.text = species[i].sensorAngle.ToString("N2");
                labelElements[i].sensorAngleOffset.text = species[i].sensorOffset.ToString("N2");
            }
        }

        public void instantiateDataDisplays(int speciesAmount)
        {
            species = new Species[speciesAmount];
            dataDisplays = new TemplateContainer[speciesAmount];
            labelElements = new LabelElements[speciesAmount];
            
            for(int i = 0; i < speciesAmount; i++)
            {
                dataDisplays[i] = dataDisplaySource.Instantiate();

                labelElements[i] = new LabelElements()
                {
                    parent             = dataDisplays[i].Q("DataContainer"),
                    color              = dataDisplays[i].Q("Color")              as Label,
                    spawnPosition      = dataDisplays[i].Q("SpawnPosition")      as Label,
                    population         = dataDisplays[i].Q("Population")         as Label,
                    alivePopulation    = dataDisplays[i].Q("AlivePopulation")    as Label,
                    hungerAccumulation = dataDisplays[i].Q("HungerAccumulation") as Label,
                    turnSpeed          = dataDisplays[i].Q("TurnSpeed")          as Label,
                    sensorAngle        = dataDisplays[i].Q("SensorAngle")        as Label,
                    sensorAngleOffset  = dataDisplays[i].Q("SensorAngleOffset")  as Label
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