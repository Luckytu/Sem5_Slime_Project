namespace Slime_UI
{
    public class CameraSettingsInGameUI : UIGraphElement
    {
        private void Start()
        {
            root.visible = false;
        }

        public override void enable()
        {
            root.visible = true;
        }
    
        public override void onDisable()
        {
            throw new System.NotImplementedException();
        }

        protected override void fallback()
        {
            throw new System.NotImplementedException();
        }
    }
}
