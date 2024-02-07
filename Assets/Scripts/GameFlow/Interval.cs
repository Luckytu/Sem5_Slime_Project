using System;
using System.Collections;
using Global;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace GameFlow
{
    public class Interval : MonoBehaviour
    {
        [SerializeField] protected Timeline timeline;
        
        [SerializeField] protected float duration;
        public float remaining;

        [SerializeField] protected float randomVariance;
        
        [SerializeField] protected Interval next;
        [SerializeField] protected bool loopThis;
        protected bool paused;

        [SerializeField] protected UnityEvent onStart;
        [SerializeField] protected UnityEvent onFinished;

        protected void Update()
        {
            if (GameState.state == GameState.Paused)
            {
                pause();
            }

            if (GameState.state == GameState.Simulation)
            {
                resume();
            }
        }

        public void startInterval()
        {
            if (timeline != null)
            {
                timeline.setActive(this);
            }
            
            remaining = duration + Random.Range(-randomVariance, randomVariance);
            onStart.Invoke();
            StartCoroutine(interval());
        }

        protected virtual IEnumerator interval()
        {
            while (remaining > 0)
            {
                yield return new WaitWhile(() => paused);
                remaining -= Time.deltaTime;
                yield return null;
            }
            
            onFinished.Invoke();

            if (loopThis)
            {
                startInterval();
                yield break;
            }

            if (next != null)
            {
                next.startInterval();
            }
        }

        public void finish()
        {
            remaining = 0;
        }
        
        public void pause()
        {
            paused = true;
        }

        public void resume()
        {
            paused = false;
        }
    }
}