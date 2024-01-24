using UnityEngine;
using UnityEngine.UIElements;

namespace Slime_UI
{
    [RequireComponent(typeof(UIDocument))]

    public abstract class UIGraphElement : MonoBehaviour
    {
        protected Transform uiBackGround { get; } = UIProperties.uiBackground;
        protected UIDocument uiDocument { get; set; }
        protected VisualElement root { get; set; }
        protected bool active { get; set; }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                fallback();
            }
        }

        protected virtual void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;
            root.visible = false;
        }

        public abstract void onDisable();

        public abstract void enable();

        protected abstract void fallback();
    }
}
