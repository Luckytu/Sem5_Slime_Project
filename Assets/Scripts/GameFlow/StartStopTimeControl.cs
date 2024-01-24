using System;
using UnityEngine;

namespace GameFlow
{
    public class StartStopTimeControl : GameFlowControl
    {
        [SerializeField] private float interval;
        [SerializeField] private float randomIntervalVariance;
        private float currentInterval;

        [SerializeField] private float duration;
        [SerializeField] private float randomDurationVariance;
        private float currentDuration;
        
        private float currentTime;

        private void Update()
        {
            
        }

        protected override void executeGameFlow()
        {
            throw new System.NotImplementedException();
        }
    }
}