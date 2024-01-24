using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFlow
{
    public class GameFlowController : MonoBehaviour
    {
        public ScriptableObject[] controls;

        public UnityEvent[] events;
        
        private void Update()
        {
            foreach (ScriptableObject control in controls)
            {
                
            }
        }
    }
}