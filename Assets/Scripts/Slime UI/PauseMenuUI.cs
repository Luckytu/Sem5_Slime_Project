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
            back.RegisterCallback<ClickEvent>(backButtonClick);

            quit = root.Q("QuitGameButton") as Button;
            quit.RegisterCallback<ClickEvent>(quitButtonClick);

            simulationSettings = root.Q("SimulationSettingsButton") as Button;

            cameraSettings = root.Q("CameraSettingsButton") as Button;
            cameraSettings.RegisterCallback<ClickEvent>(cameraSettingsClick);
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                root.visible = !root.visible;
            }
        }

        private void backButtonClick(ClickEvent e)
        {
            onDisable();
        }

        private void quitButtonClick(ClickEvent e)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }

        private void cameraSettingsClick(ClickEvent e)
        {
            onDisable();

            cameraSettingsUI.enable();
        }

        public override void onDisable()
        {
            root.visible = false;
        }

        public override void enable()
        {
            root.visible = true;
        }

        protected override void fallback()
        {
            throw new System.NotImplementedException();
        }
    }
}
