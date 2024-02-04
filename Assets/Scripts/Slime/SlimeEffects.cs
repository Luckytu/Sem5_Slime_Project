using System;
using Slime.Effects;
using UnityEngine;

namespace Slime
{
    public class SlimeEffects : Simulation
    {
        [SerializeField] private SpeciesDataDisplay speciesDataDisplay;
        [SerializeField] private int entityReadAmount;
        
        private Species[] species;

        private void Update()
        {
            if (species != null)
            {
                speciesDataDisplay.species = species;
            }
        }

        private void FixedUpdate()
        {
            if (species != null)
            {
                simulationSettings.speciesBuffer.GetData(species, 0, 0, simulationSettings.currentSpeciesAmount);
            }
        }

        public void initializeSpecies()
        {
            species = new Species[simulationSettings.currentSpeciesAmount];
            
            speciesDataDisplay.instantiateDataDisplays(species);
        }
    }
}