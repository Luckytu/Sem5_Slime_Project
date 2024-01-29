using System;
using Global;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameFlow
{
    public class StartStopTimeControl : GameFlowControl
    {
        private enum State
        {
            Interval,
            Execution
        }
        
        [SerializeField] private float interval;
        [SerializeField] private float randomIntervalVariance;
        public float currentInterval;

        [SerializeField] private float duration;
        [SerializeField] private float randomDurationVariance;
        public float currentDuration;

        private State state = State.Interval;
        
        private void Start()
        {
            setRandomTimes();
        }

        private void Update()
        {
            if (GameState.state != GameState.Simulation)
            {
                return;
            }

            if (state == State.Interval)
            {
                if (currentInterval > 0)
                {
                    currentInterval -= Time.deltaTime;
                }
                else
                {
                    execute();
                }
            }
            else
            {
                if (currentDuration > 0)
                {
                    currentDuration -= Time.deltaTime;
                }
                else
                {
                    onExecutionFinished.Invoke();
                    setRandomTimes();
                    state = State.Interval;
                }
            }

        }

        protected override void executeGameFlow()
        {
            setRandomTimes();
            state = State.Execution;
        }

        private void setRandomTimes()
        {
            currentInterval = interval + Random.Range(-randomIntervalVariance, randomIntervalVariance);
            currentDuration = duration + Random.Range(-randomDurationVariance, randomDurationVariance);
        }
    }
}