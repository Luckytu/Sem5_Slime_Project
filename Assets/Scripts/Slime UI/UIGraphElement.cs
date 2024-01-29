using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Slime_UI
{
    [RequireComponent(typeof(UIDocument))]

    public abstract class UIGraphElement : MonoBehaviour
    {
        [SerializeField] protected UnityEvent onEnable;
        [SerializeField] protected UnityEvent onDisable;
        
        protected Transform uiBackGround { get; } = UIProperties.uiBackground;
        protected UIDocument uiDocument { get; set; }
        protected VisualElement root { get; set; }
        protected bool active { get; set; } = false;

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && active)
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

        public abstract void disable();

        public abstract void enable();

        protected abstract void fallback();
    }
}
