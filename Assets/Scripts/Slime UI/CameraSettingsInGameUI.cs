using System;
using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.Events;

namespace Slime_UI
{
    public class CameraSettingsInGameUI : UIGraphElement
    {
        [SerializeField] private UnityEvent updateCamera;
        
        [SerializeField] private CameraSettingsUI cameraSettingsUI;
        
        private void Start()
        {
            root.visible = false;
        }

        private IEnumerator waitForDisable()
        {
            onDisable.Invoke();
            
            yield return new WaitForFixedUpdate();
            
            disable();
        }

        private void FixedUpdate()
        {
            if (active)
            { 
                updateCamera.Invoke();
            }
        }

        public override void enable()
        {
            active = true;
            GameState.state = GameState.EditCameraInGame;
            root.visible = true;
            
            onEnable.Invoke();
        }
    
        public override void disable()
        {
            active = false;
            GameState.state = GameState.Paused;
            root.visible = false;
            cameraSettingsUI.enable();
        }

        protected override void fallback()
        {
            StartCoroutine(waitForDisable());
        }
    }
}