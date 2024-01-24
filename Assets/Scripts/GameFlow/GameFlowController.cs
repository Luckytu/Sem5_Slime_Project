using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFlow
{
    public class GameFlowController : MonoBehaviour
    {
        public ScriptableObject[] controls;
        
        private void Update()
        {
            foreach (ScriptableObject control in controls)
            {
                
            }
        }
    }
}