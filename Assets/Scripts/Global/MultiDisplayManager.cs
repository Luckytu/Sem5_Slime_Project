using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Global
{
    public class MultiDisplayManager : MonoBehaviour
    {
        private void Start()
        {
            Display.displays[1].Activate();
            Display.displays[2].Activate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Screen.fullScreen)
                {
                    Screen.fullScreen = false;
                }
                else
                {
                    Screen.fullScreen = true;
                    Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                }
            }
        }
    }
}