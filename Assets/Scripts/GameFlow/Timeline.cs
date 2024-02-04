using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    public class Timeline : MonoBehaviour
    {
        [SerializeField] private Interval first;
        [SerializeField] private bool startImmediately;
        private Interval current;

        private void Start()
        {
            if (startImmediately)
            {
                startTimeLine();
            }
        }

        public void startTimeLine()
        {
            current = first;
            first.startInterval();
        }

        public void setCurrent(Interval interval)
        {
            current = interval;
        }

        public void skipCurrent()
        {
            current.finish();
        }
        
        public void pause()
        {
            current.pause();
        }

        public void resume()
        {
            current.resume();
        }
    }
}