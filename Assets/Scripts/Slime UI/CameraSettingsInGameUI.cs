using System;
using System.Collections;
using System.Collections.Generic;
using Camera_Capture;
using Global;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Slime_UI
{
    public class CameraSettingsInGameUI : UIGraphElement
    {
        [SerializeField] private UnityEvent updateCamera;
        
        [SerializeField] private CameraSettingsUI cameraSettingsUI;
        [SerializeField] private CameraCapture cameraCapture;

        [SerializeField] private CameraCaptureSettings cameraCaptureSettings;

        private Slider x1;
        private Slider y1;
        
        private Slider x2;
        private Slider y2;

        private Slider x3;
        private Slider y3;

        private Slider x4;
        private Slider y4;

        private Slider filterCutoff;
        
        private void Start()
        {
            root.visible = false;

            x1 = root.Q("x1") as Slider;
            y1 = root.Q("y1") as Slider;
            
            x2 = root.Q("x2") as Slider;
            y2 = root.Q("y2") as Slider;

            x3 = root.Q("x3") as Slider;
            y3 = root.Q("y3") as Slider;

            x4 = root.Q("x4") as Slider;
            y4 = root.Q("y4") as Slider;

            filterCutoff = root.Q("filterCutoffSlider") as Slider;
            
            x1?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.x1 = e.newValue);
            y1?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.y1 = e.newValue);
            
            x2?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.x2 = e.newValue);
            y2?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.y2 = e.newValue);

            x3?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.x3 = e.newValue);
            y3?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.y3 = e.newValue);

            x4?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.x4 = e.newValue);
            y4?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.p.y4 = e.newValue);

            filterCutoff?.RegisterCallback<ChangeEvent<float>>((e) => cameraCaptureSettings.filterCutoff = e.newValue);
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
            
            cameraCapture.showFiltered = true;
            cameraCapture.updateConstantly = true;
            
            x1.value = cameraCaptureSettings.p.x1;
            y1.value = cameraCaptureSettings.p.y1;

            x2.value = cameraCaptureSettings.p.x2;
            y2.value = cameraCaptureSettings.p.y2;

            x3.value = cameraCaptureSettings.p.x3;
            y3.value = cameraCaptureSettings.p.y3;

            x4.value = cameraCaptureSettings.p.x4;
            y4.value = cameraCaptureSettings.p.y4;

            filterCutoff.value = cameraCaptureSettings.filterCutoff;
            
            onEnable.Invoke();
        }
    
        public override void disable()
        {
            active = false;
            GameState.state = GameState.Paused;
            root.visible = false;
            cameraSettingsUI.enable();
            
            cameraCapture.showFiltered = true;
            cameraCapture.updateConstantly = false;
        }

        protected override void fallback()
        {
            StartCoroutine(waitForDisable());
        }
    }
}