using Global;
using UnityEngine;
using UnityEngine.UIElements;

namespace Slime_UI
{
    public class PauseMenuUI : UIGraphElement
    {
        [SerializeField] private UIGraphElement cameraSettingsUI;
        
        private Button back;
        private Button quit;
        private Button simulationSettings;
        private Button cameraSettings;

        private void Start()
        {
            root.visible = false;

            back = root.Q("BackButton") as Button;
            back?.RegisterCallback<ClickEvent>(backButtonClick);

            quit = root.Q("QuitGameButton") as Button;
            quit?.RegisterCallback<ClickEvent>(quitButtonClick);

            simulationSettings = root.Q("SimulationSettingsButton") as Button;

            cameraSettings = root.Q("CameraSettingsButton") as Button;
            cameraSettings?.RegisterCallback<ClickEvent>(cameraSettingsClick);
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameState.state == GameState.Simulation)
                {
                    enable();
                }
                else
                {
                    fallback();
                }
            }
        }

        private void backButtonClick(ClickEvent e)
        {
            disable();
        }

        private void quitButtonClick(ClickEvent e)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }

        private void cameraSettingsClick(ClickEvent e)
        {
            disable();

            cameraSettingsUI.enable();
        }

        public override void enable()
        {
            active = true;
            GameState.state = GameState.Paused;
            root.visible = true;
        }
        
        public override void disable()
        {
            active = false;
            GameState.state = GameState.Simulation;
            root.visible = false;
        }
        
        protected override void fallback()
        {
            disable();
        }
    }
}
