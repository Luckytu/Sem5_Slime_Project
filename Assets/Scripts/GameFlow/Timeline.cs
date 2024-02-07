using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    public class Timeline : MonoBehaviour
    {
        [SerializeField] private Interval first;
        [SerializeField] private bool startImmediately;
        private Interval active;

        private void Start()
        {
            if (startImmediately)
            {
                startTimeLine();
            }
        }

        public void startTimeLine()
        {
            setActive(first);
            first.startInterval();
        }

        public void setActive(Interval interval)
        {
            active = interval;
        }
        
        public void skipActives()
        {
            active.finish();
        }
        
        public void pause()
        {
            active.pause();
        }

        public void resume()
        {
            active.resume();
        }
    }
}